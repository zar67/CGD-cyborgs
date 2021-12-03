using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject leaderBoard;
    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {

    }

    public void ShowLeaderBoards()
    {
        leaderBoard.SetActive(true);
        this.gameObject.SetActive(false);
	}

    public void loadLobby()
    {
        SceneManager.LoadScene("CombatScene");
    }
}
