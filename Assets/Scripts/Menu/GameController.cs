using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{ 
    public string playerID;
    public string serverIp;
    public int serverPort;
    public string serverGameId;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void connectToServer(ConnectHTTPServerInfo serverInfo)
    {
        serverGameId = serverInfo.game_id;
        serverIp = serverInfo.host;
        serverPort = serverInfo.port;
        SceneManager.LoadScene("Game");
    }
}
