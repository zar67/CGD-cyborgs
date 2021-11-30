using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelected : MonoBehaviour
{

    public GameObject gameObjectUiUnitSelect;
    public GameObject gameObjectUnitPortrait;
    public SpriteRenderer spriteRenderer;
    public Image m_Image;

    private int health
    {
        get { return health; }
        set { health = value;}
    }
    public Text healthText;
    public Text movementText;
    public Text sightText;
    public Text attackText;
    public Text nameText;
    public Text confirmedKills;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    // Update is called once per frame
    void Update()
    {

        if(WorldSelection.SelectedObject is Unit unit)
        {
            //Debug.Log("TRUE");
            gameObjectUiUnitSelect.SetActive(true);
            gameObjectUnitPortrait.SetActive(true);

            healthText.text = "HP: " + unit.GetHealth();
            movementText.text = "Speed: " + unit.GetMovementSpeed();
            sightText.text = "Sight: " + unit.GetSight();
            attackText.text = "Attack: " + unit.GetDamage();
            nameText.text = "" + unit.GetCurrentUnitType();
            m_Image.sprite = unit.GetComponent<SpriteRenderer>().sprite;
        }

        if (WorldSelection.SelectedObject is Ruin ruin)
        {
            gameObjectUiUnitSelect.SetActive(false);
            gameObjectUnitPortrait.SetActive(true);
            Debug.Log("Ruin");
        }

     else if (WorldSelection.SelectedObject == null)
        {
            //Debug.Log("FALSE");
            gameObjectUiUnitSelect.SetActive(false);
            gameObjectUnitPortrait.SetActive(false);
        }
    }



}