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

// Using for sending information about player to UDPServer
[System.Serializable]
public class PlayerData
{
    public int game_id;
    public int player_id;
    public Position position;

    public PlayerData(){}

    public PlayerData(int game_id, int player_id, Position position) 
    {
        this.game_id = game_id;
        this.player_id = player_id;
        this.position = position;
    }
}

// Using for describing enemys
[System.Serializable]
public class EnemyData
{
    public int enemy_id;
    public Position position;

    public EnemyData() {}

    public EnemyData(int enemy_id, Position position)
    {
        this.enemy_id = enemy_id;
        this.position = position;
    }
}

// Using for reciving information about server status and enemys positions
[System.Serializable]
public class ServerData
{
    public string status;
    public EnemyData[] enemys;

    public ServerData() {}

    public ServerData(string status, EnemyData[] enemys)
    {
        this.status = status;
        this.enemys = enemys;
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
    public int game_id;
    public string host;
    public int port;

    public ConnectHTTPServerInfo(){}

    public ConnectHTTPServerInfo(int game_id, string host, int port)
    {
        this.game_id = game_id;
        this.host = host;
        this.port = port;
    }
}

