using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IDLabel : MonoBehaviour
{
    private Text id_text;

    void Start()
    {
        id_text = GetComponent<Text>();
    }

    public void ChangedID(string text)
    {
        //FindObjectOfType<GameController>().playerID = id_text.text;
    }
}
