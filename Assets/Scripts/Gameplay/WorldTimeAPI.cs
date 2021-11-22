using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

public class WorldTimeAPI : MonoBehaviour
{
    private const string API_URL = "https://worldtimeapi.org/api/timezone/gmt";
    private static JSONNode jsonData;

    public static int GetCurrentDay()
    {
        if (jsonData.IsNull)
        {
            Debug.Log("Error: jsonData is null");
            return -1;
        }

        return jsonData["day_of_year"].AsInt;
    }

    private void Awake()
    {
        GenerateRequest();
    }

    private void GenerateRequest()
    {
        StartCoroutine(ProcessRequest(API_URL));
    }

    IEnumerator ProcessRequest(string uri)
    {
        UnityWebRequest request = UnityWebRequest.Get(API_URL);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(request.error);
        }
        else
        {
            string data = request.downloadHandler.text;
            jsonData = JSON.Parse(data);

            if(!jsonData.IsNull)
            {
                Debug.Log("JSON data loaded successfully");
            }
            else
            {
                Debug.Log("Failed to obtain JSON data");
            }
        }
    }
}