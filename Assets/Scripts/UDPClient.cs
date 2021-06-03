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
    private string game_id;
    private string host;
    private int port;
    private string player_id;

    private string server_status;

    private Dictionary<string, Enemy> enemys;
    private Dictionary<string, Vector3> enemys_data;

    private Thread receive_thread;
    private IPEndPoint remote_end_point;
    private UdpClient client;

    public PlayController player;
    public GameObject enemy_prefab;

    void Start()
    {
        enemys_data = new Dictionary<string, Vector3>();
        enemys = new Dictionary<string, Enemy>();

        server_status = "waiting";

        ProcessGameControllerData();
        InitUdpClient();
    }

    private void Update()
    {
        SendPlayerInfo();
        checkAndCreateEnemy();
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

        ServerRecieveData server_rec_data = new ServerRecieveData();
        server_rec_data.Deserialize(SimpleJSON.JSON.Parse(row_data_str));

        ServerParams server_data = server_rec_data.parameters;
        
        server_status = server_data.stage;

        foreach (KeyValuePair<string, PlayerData> enemy_info in server_data.players)
        {
            if (enemy_info.Value.player_id == player_id)
            {
                continue;
            }
            enemys_data[enemy_info.Value.player_id] = new Vector3(enemy_info.Value.position.x,
                                                                  enemy_info.Value.position.y,
                                                                  enemy_info.Value.position.z);
        }
    }

    void updateEnemys()
    {
        if (server_status == "running")
        {
            foreach (KeyValuePair<string, Enemy> enemy in enemys)
            {
                enemy.Value.Move(enemys_data[enemy.Key]);
            }
        }
    }

    void checkAndCreateEnemy()
    {
        foreach (KeyValuePair<string, Vector3> enemy_info in enemys_data)
        {
            if (!enemys.ContainsKey(enemy_info.Key))
            {
                GameObject enemy_object = Instantiate(enemy_prefab, enemy_info.Value, Quaternion.identity);
                enemys[enemy_info.Key] = enemy_object.GetComponent<Enemy>();
            }
        }
    }

    void SendPlayerInfo()
    {
        Vector3 pos = player.transform.position;
        ClientParams client_params = new ClientParams(
            game_id, player_id, "test_name", new Position(pos.x, pos.y, pos.z));

        ClientSendData client_send_data = new ClientSendData("set_position", client_params);

        byte[] bytes_to_send = Encoding.UTF8.GetBytes(JsonUtility.ToJson(client_send_data));

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
