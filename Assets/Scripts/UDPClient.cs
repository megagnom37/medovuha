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
    public string IP = "127.0.0.1";
    public int port = 9999;
    public PlayController player;
    public Enemy[] enemys;

    Thread receiveThread;
    Thread sendThread;
    IPEndPoint remoteEndPoint;
    UdpClient client;

    Dictionary<int, Vector3> enemysData;

    void Start()
    {
        enemysData = new Dictionary<int, Vector3>();
        init();
    }

    private void Update()
    {
        SendPosition();
        updateEnemys();
    }

    public void init()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
        //sendString($"{player.playerID}:0:0:0");

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

        //StartCoroutine("SendPosition");
    }

    private void ReceiveData()
    {
        print("Recieve Started");
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
        string rowDataStr = Encoding.UTF8.GetString(data);
        string[] playersDataStr = rowDataStr.Split(';');
        foreach (var playerDataStr in playersDataStr)
        {
            string[] player_info = playerDataStr.Split(':');
            int playerId = Int32.Parse(player_info[0]);
            if (playerId == player.playerID)
            {
                continue;
            }
            enemysData[playerId] = new Vector3(float.Parse(player_info[1]),
                                               float.Parse(player_info[2]),
                                               float.Parse(player_info[3]));
        }
    }

    void updateEnemys()
    {
        foreach (var enemy in enemys)
        {
            if (enemysData.ContainsKey(enemy.playerID))
            {
                enemy.Move(enemysData[enemy.playerID]);
            } 
        }
    }

    //IEnumerator SendPosition()
    //{
    //    while (true)
    //    {
    //        Vector3 pos = player.transform.position;
    //        string udpMessageStr = $"{player.playerID}:{pos.x}:{pos.y}:{pos.z}";
    //        sendString(udpMessageStr);
    //        yield return new WaitForSeconds(0.1f);
    //    }
    //}

    void SendPosition()
    {
        Vector3 pos = player.transform.position;
        string udpMessageStr = $"{player.playerID}:{pos.x}:{pos.y}:{pos.z}";
        sendString(udpMessageStr);
    }

    private void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }
    
}
