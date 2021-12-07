using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject leaderBoard;
    [SerializeField] GameObject audioSliders;
    [SerializeField] GameObject menuB1;
    [SerializeField] GameObject menuB2;
    [SerializeField] GameObject menuB3;
    [SerializeField] GameObject menuB4;
    [SerializeField] GameObject menuB5;
    Slider sliderSFX;
    Slider sliderMusic;


    public void ShowLeaderBoards()
    {
        leaderBoard.SetActive(true);
        gameObject.SetActive(false);
	}

    public void ShowOptions()
    {
            DisableMenuButtons(); 
    }

    public void DisableMenuButtons()
    {
        audioSliders.SetActive(true);
        menuB1.SetActive(false);
            menuB2.SetActive(false);
            menuB3.SetActive(false);
            menuB4.SetActive(false);
        
            menuB5.SetActive(true);

    }


    public void EnableMenuButtons()
    {

        audioSliders.SetActive(false);
        menuB1.SetActive(true);
            menuB2.SetActive(true);
            menuB3.SetActive(true);
            menuB4.SetActive(true);

        menuB5.SetActive(false);

    }

}
