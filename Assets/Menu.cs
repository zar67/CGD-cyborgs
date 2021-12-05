using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject leaderBoard;

    public void ShowLeaderBoards()
    {
        leaderBoard.SetActive(true);
        gameObject.SetActive(false);
	}
}
