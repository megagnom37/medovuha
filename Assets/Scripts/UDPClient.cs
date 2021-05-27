using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDPClient : MonoBehaviour
{
    private int game_id;
    private string host;
    private int port;
    private int player_id;

    private string server_status;

    private Dictionary<int, Enemy> enemys;
    private Dictionary<int, Vector3> enemys_data;

    private Thread receive_thread;
    private IPEndPoint remote_end_point;
    private UdpClient client;

    public PlayController player;
    public GameObject enemy_prefab;

    void Start()
    {
        enemys_data = new Dictionary<int, Vector3>();
        enemys = new Dictionary<int, Enemy>();

        server_status = "waiting";

        ProcessGameControllerData();
        InitUdpClient();
    }

    private void Update()
    {
        SendPlayerInfo();
        updateEnemys();
    }

    private void ProcessGameControllerData()
    {
        GameController game_crtl = GameObject.FindObjectOfType<GameController>();
        game_id = game_crtl.serverGameId;
        host = game_crtl.serverIp;
        port = game_crtl.serverPort;
        player_id = game_crtl.playerID;
    }

    public void InitUdpClient()
    {
        remote_end_point = new IPEndPoint(IPAddress.Parse(host), port);
        client = new UdpClient();

        receive_thread = new Thread(
            new ThreadStart(ReceiveData));
        receive_thread.IsBackground = true;
        receive_thread.Start();
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] dataBytes = client.Receive(ref anyIP);
                updateEnemysData(dataBytes);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    void updateEnemysData(byte[] data)
    {
        string row_data_str = Encoding.UTF8.GetString(data);
        ServerData server_data = JsonUtility.FromJson<ServerData>(row_data_str);
        server_status = server_data.status;

        foreach (var enemy_info in server_data.enemys)
        {
            if (enemy_info.enemy_id == player_id)
            {
                continue;
            }
            enemys_data[enemy_info.enemy_id] = new Vector3(enemy_info.position.x,
                                                           enemy_info.position.y,
                                                           enemy_info.position.z);
        }

        if (server_status == "waiting")
        {
            foreach (KeyValuePair<int, Vector3> enemy_info in enemys_data)
            {
                if (!enemys.ContainsKey(enemy_info.Key))
                {
                    GameObject enemy_object = Instantiate(enemy_prefab, enemy_info.Value, Quaternion.identity);
                    enemys[enemy_info.Key] = enemy_object.GetComponent<Enemy>();
                }
            }
        }
    }

    void updateEnemys()
    {
        if (server_status == "running")
        {
            foreach (KeyValuePair<int, Enemy> enemy in enemys)
            {
                enemy.Value.Move(enemys_data[enemy.Key]);
            }
        }
    }

    void SendPlayerInfo()
    {
        Vector3 pos = player.transform.position;
        PlayerData player_data = new PlayerData(
            game_id, player_id, new Position(pos.x, pos.y, pos.z));
        byte[] bytes_to_send = Encoding.UTF8.GetBytes(JsonUtility.ToJson(player_data));

        try
        {
            client.Send(bytes_to_send, bytes_to_send.Length, remote_end_point);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
}
