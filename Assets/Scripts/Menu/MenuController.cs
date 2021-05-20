using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MenuController : MonoBehaviour
{
    [System.Serializable]
    public class ServerUrl
    {
        public string host;
        public int port;       
    }

    public GameController gameCtrl;
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClickPlayButton()
    {
        StartCoroutine(PostRequest("http://127.0.0.1:8080/server_connector/connect"));
    }
    IEnumerator PostRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, ""))
        {
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    ServerUrl serverUrl = JsonUtility.FromJson<ServerUrl>(webRequest.downloadHandler.text);
                    gameCtrl.connectToServer(serverUrl.host, 9999); //TODO: set normal port
                    
                    break;
            }
        }
    }

}
