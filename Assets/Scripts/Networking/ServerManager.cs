using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void SetScore(string _name, string _score)
    {
         StartCoroutine(GetRequest("http://liam0neale.pythonanywhere.com/SetScore/" + _name + "/" + _score));
	}

    public void IncrementScore(string _name, string _score)
    {
        StartCoroutine(GetRequest("http://liam0neale.pythonanywhere.com/IncrementScore/" + _name + "/" + _score));
	}

    public void DecrementScore(string _name, string _score)
    {
        StartCoroutine(GetRequest("http://liam0neale.pythonanywhere.com/DecrementScore/" + _name + "/" + _score));
	}

    public void FillLeaderBoards()
    {
        
	}
     IEnumerator GetRequest(string uri)
     {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
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
                    break;
            }
        }
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", "harry");
        form.AddField("score", "175");

        using (UnityWebRequest www = UnityWebRequest.Post("http://liam0neale.pythonanywhere.com/SetScore/name/score", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
}
