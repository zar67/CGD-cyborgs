using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderBoardItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Awake()
    {
        nameText = gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        scoreText = gameObject.transform.Find("Score").GetComponent<TextMeshProUGUI>();
    }
}
