using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using for describing object position
[System.Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;

    public Position(){}

    public Position(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

[System.Serializable]
public class ClientSendData
{
    public string method;
    public ClientParams parameters;

    public ClientSendData() { }

    public ClientSendData(string method, ClientParams parameters)
    {
        this.method = method;
        this.parameters = parameters;
    }
}

[System.Serializable]
public class ClientParams
{
    public string game_id;
    public string player_id;
    public string player_name;
    public Position position;

    public ClientParams() { }

    public ClientParams(string game_id, string player_id, string player_name, Position position)
    {
        this.game_id = game_id;
        this.player_id = player_id;
        this.player_name = player_name;
        this.position = position;
    }
}

// Using for sending information about player to UDPServer
[System.Serializable]
public class PlayerData
{
    public string player_id;
    public string player_name;
    public Position position;

    public PlayerData(){}

    public PlayerData(string player_name, string player_id, Position position) 
    {
        this.player_name = player_name;
        this.player_id = player_id;
        this.position = position;
    }
}

[System.Serializable]
public class ServerRecieveData
{
    public string method;
    public ServerParams parameters;

    public ServerRecieveData() { }

    public ServerRecieveData(string method, ServerParams parameters)
    {
        this.method = method;
        this.parameters = parameters;
    }
}

// Using for reciving information about server status and enemys positions
[System.Serializable]
public class ServerParams
{
    public string stage;
    public ConnectHTTPServerInfo info;
    public Dictionary<string, PlayerData> players;

    public ServerParams() {}

    public ServerParams(string stage, ConnectHTTPServerInfo info, Dictionary<string, PlayerData> players)
    {
        this.stage = stage;
        this.info = info;
        this.players = players;
    }
}

// Data about player for sending to HTTP server
[System.Serializable]
public class ConnectHTTPPlayerInfo
{
    public int player_id;

    public ConnectHTTPPlayerInfo(){}

    public ConnectHTTPPlayerInfo(int player_id)
    {
        this.player_id = player_id;
    }
}

// Data about UDPServer reciving from HTTP Server
[System.Serializable]
public class ConnectHTTPServerInfo
{
    public string game_id;
    public string host;
    public int port;

    public ConnectHTTPServerInfo(){}

    public ConnectHTTPServerInfo(string game_id, string host, int port)
    {
        this.game_id = game_id;
        this.host = host;
        this.port = port;
    }
}

