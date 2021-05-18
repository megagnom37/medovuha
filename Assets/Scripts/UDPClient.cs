using UnityEngine;
using System.Collections;

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

    Thread receiveThread;
    IPEndPoint remoteEndPoint;
    UdpClient client;

    void Start()
    {
        init();
        StartCoroutine("SendPosition");
    }

    public void init()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
        sendString($"{player.playerID}:0:0:0");

        receiveThread = new Thread(
            new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        print("Recieve Started");
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                print(">> " + text);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    IEnumerator SendPosition()
    {
        while (true)
        {
            Vector3 pos = player.transform.position;
            string udpMessageStr = $"{player.playerID}:{pos.x}:{pos.y}:{pos.z}";
            sendString(udpMessageStr);
            yield return new WaitForSeconds(2f);
        }
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
