using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class MenuController : MonoBehaviour
{
    public GameController gameCtrl;
    

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ClickPlayButton()
    {
        StartCoroutine(GetRequest("http://127.0.0.1:8080/get_game"));
    }
    IEnumerator GetRequest(string uri)
    {
        //ConnectHTTPPlayerInfo postData = new ConnectHTTPPlayerInfo(gameCtrl.playerID);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
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
                    ConnectHTTPServerInfo serverInfo = JsonUtility.FromJson<ConnectHTTPServerInfo>(
                        webRequest.downloadHandler.text);
                    gameCtrl.connectToServer(serverInfo);
                    
                    break;
            }
        }
    }
}
