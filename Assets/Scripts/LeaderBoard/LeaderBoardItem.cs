using TMPro;
using UnityEngine;

public class LeaderBoardItem : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    // Start is called before the first frame update
    private void Awake()
    {
        nameText = gameObject.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        scoreText = gameObject.transform.Find("Score").GetComponent<TextMeshProUGUI>();
    }
}
