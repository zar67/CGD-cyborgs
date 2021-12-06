using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelected : MonoBehaviour
{
    [Serializable]
    public struct RuinSelectAction
    {
        public Unit.UnitTypes UnitType;
        public Button SelectButton;
        public Image SelectedImage;
    }

    [SerializeField] private List<RuinSelectAction> m_unitTypeActions = new List<RuinSelectAction>();

    public GameObject gameObjectUiUnitSelect;
    public GameObject gameObjectUnitPortrait;
    public GameObject gameObjectUiRuinSelect;
    public Image m_Image;

    private int health
    {
        get { return health; }
        set { health = value;}
    }
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI movementText;
    public TextMeshProUGUI sightText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI nameText;

    private void Awake()
    {
        foreach (RuinSelectAction action in m_unitTypeActions)
        {
            action.SelectedImage.enabled = action.UnitType == Unit.UnitTypes.SOLDIER;
            action.SelectButton.interactable = action.UnitType != Unit.UnitTypes.SOLDIER;
            action.SelectButton.onClick.AddListener(delegate
            {
                ChangeRuinUnitType(action.UnitType);
            });
        }
    }

    void Update()
    {
        HUD();
    }

    public void HUD()
    {
        if (WorldSelection.SelectedObject is Unit unit)
        {
            gameObjectUiUnitSelect.SetActive(true);
            gameObjectUnitPortrait.SetActive(true);
            gameObjectUiRuinSelect.SetActive(false);

            healthText.text = unit.GetHealth().ToString() + "/" + UnitFactory.Instance.GetBaseUnitStats(unit.Type).health;
            movementText.text = unit.Movement.ToString() + "/" + unit.GetMovementSpeed().ToString();
            sightText.text = unit.GetSight().ToString();
            attackText.text = unit.GetDamage().ToString();
            nameText.text = unit.GetCurrentUnitType().ToString();
            m_Image.sprite = unit.GetComponent<SpriteRenderer>().sprite;
            m_Image.SetNativeSize();
        }

        if (WorldSelection.SelectedObject is Ruin ruin)
        {
            gameObjectUiUnitSelect.SetActive(false);
            gameObjectUiRuinSelect.SetActive(true);
            gameObjectUnitPortrait.SetActive(true);
            m_Image.sprite = ruin.GetComponent<SpriteRenderer>().sprite;
        }

        else if (WorldSelection.SelectedObject == null)
        {
            gameObjectUiUnitSelect.SetActive(false);
            gameObjectUnitPortrait.SetActive(false);
            gameObjectUiRuinSelect.SetActive(false);
        }
    }

    private void ChangeRuinUnitType(Unit.UnitTypes type)
    {
        if (WorldSelection.SelectedObject is Ruin ruin)
        {
            ruin.UnitType = type;
  
            foreach (RuinSelectAction action in m_unitTypeActions)
            {
                action.SelectedImage.enabled = action.UnitType == type;
                action.SelectButton.interactable = action.UnitType != type;
                FindObjectOfType<SoundManager>().Play("Button");
            }

            XMLFormatter.AddUnitTypeChange(ruin, type);
            ruin.RespawnUnit();
        }
    }
}