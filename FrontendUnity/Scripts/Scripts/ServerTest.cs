using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ServerTest : MonoBehaviour
{
    public TextMeshPro textDisplay;

    void Start()
    {
        textDisplay.text = "Start!";
        GetData();
    }

    void GetData() => StartCoroutine(GetData_Coroutine());

    IEnumerator GetData_Coroutine()
    {
        textDisplay.text = "Loading...";
        string url = "http://10.10.24.212:3000/get-model";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
                textDisplay.text = request.error;
            else
            {
                string response = request.downloadHandler.text;
                textDisplay.text = response;
            }
        }
    }
}