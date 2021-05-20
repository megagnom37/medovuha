using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public int playerID;
    public string serverIp;
    public int serverPort;

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

    public void connectToServer(string ip, int port)
    {
        serverIp = ip;
        serverPort = port;
        SceneManager.LoadScene("Game");
    }
}
