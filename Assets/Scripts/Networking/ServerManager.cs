using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    [SerializeField] GameObject prefabItem;
    [SerializeField] GameObject contents;
    [SerializeField] GameObject mainMenu;

    // Start is called before the first frame update
    void Start()
    {
       FillLeaderBoards();
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

    public void MainMenu()
    {
        mainMenu.SetActive(true);
        gameObject.SetActive(false);
	}

    public void Resfresh()
    {
        FillLeaderBoards();
	}

    public void FillLeaderBoards()
    {
         StartCoroutine(BuildLeaderBoards());
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

    IEnumerator BuildLeaderBoards()
     {
        string uri = "http://liam0neale.pythonanywhere.com/GetAllScores";
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

                    foreach (Transform child in contents.transform)
                    {
                        Destroy(child.gameObject);
                    }

                    string formatted = webRequest.downloadHandler.text.Replace("{", "");
                    formatted = formatted.Replace("}", "");

                    var playerNames = new List<string>();
                    var playerScores = new List<int>();
                    foreach (string player in formatted.Split(','))
                    {
                        string[] playerSplit = player.Split(':');
                        playerNames.Add(playerSplit[0].Replace("'", "").Trim());
                        playerScores.Add(int.Parse(playerSplit[1].Replace("'", "").Trim()));
                    }

                    string[] namesArray = playerNames.ToArray();
                    int[] scoresArray = playerScores.ToArray();

                    Array.Sort(scoresArray, namesArray);

                    for (int i = namesArray.Length - 1; i >= 0; i--)
                    {
                        GameObject playerItem = Instantiate(prefabItem);
                        playerItem.transform.SetParent(contents.transform);
                        playerItem.transform.localPosition = Vector3.zero;
                        playerItem.transform.localScale = Vector3.one;

                        LeaderBoardItem itemStuff = playerItem.GetComponent<LeaderBoardItem>();
                        itemStuff.nameText.text = namesArray[i];
                        itemStuff.scoreText.text = scoresArray[i].ToString();
                    }

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
