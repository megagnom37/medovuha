using UnityEngine;
using System.Collections;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

public class UDPSend : MonoBehaviour
{
    public string IP = "127.0.0.1"; 
    public int port = 9999;
    public int playerID = 0;

    IPEndPoint remoteEndPoint;
    UdpClient client;

    public GameObject recieverUDPPrefab;

    void Start()
    {
        init();
        createRecieverUPD();
        StartCoroutine("SendPosition");
    }

    void createRecieverUPD()
    {
        UDPReceive recieverUDP = Instantiate(recieverUDPPrefab).GetComponent<UDPReceive>();
        sendString($"{playerID}:0:0:0");
        recieverUDP.port = ((IPEndPoint)client.Client.LocalEndPoint).Port;
    }

    IEnumerator SendPosition()
    {
        while (true)
        {
            Vector3 pos = transform.position;
            string udpMessageStr = $"{playerID}:{pos.x}:{pos.y}:{pos.z}";
            sendString(udpMessageStr);
            yield return new WaitForSeconds(2f);
        }
    }

    public void init()
    {
        remoteEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
        client = new UdpClient();
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