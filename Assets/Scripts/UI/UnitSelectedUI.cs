using Audio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AudioType = Audio.AudioType;

public class UnitSelectedUI : MonoBehaviour
{
    [Serializable]
    public struct RuinSelectAction
    {
        public Unit.EUnitType UnitType;
        public Button SelectButton;
        public Image SelectedImage;
    }

    [SerializeField] private List<RuinSelectAction> m_unitTypeActions = new List<RuinSelectAction>();

    [Header("Selected Unit References")]
    [SerializeField] private GameObject m_selectedUnitHolder;
    [SerializeField] private TextMeshProUGUI m_unitHealthText;
    [SerializeField] private TextMeshProUGUI m_unitMovementText;
    [SerializeField] private TextMeshProUGUI m_unitSightText;
    [SerializeField] private TextMeshProUGUI m_unitDamageText;
    [SerializeField] private TextMeshProUGUI m_unitNameText;

    [Header("Selected Object Portrait References")]
    [SerializeField] private GameObject m_selectedPortraitHolder;
    [SerializeField] private Button m_selectedPortraitButton = default;
    [SerializeField] private Image m_portraitImage;

    [Header("Selected Ruin References")]
    [SerializeField] private GameObject m_selectedRuinHolder;

    public AudioController audioController;

    private CameraController m_cameraController = default;

    private void Awake()
    {
        m_cameraController = FindObjectOfType<CameraController>();

        foreach (RuinSelectAction action in m_unitTypeActions)
        {
            action.SelectedImage.enabled = action.UnitType == Unit.EUnitType.SOLDIER;
            action.SelectButton.interactable = action.UnitType != Unit.EUnitType.SOLDIER;
            action.SelectButton.onClick.AddListener(delegate
            {
                ChangeRuinUnitType(action.UnitType);
            });
        }
    }

    private void OnEnable()
    {
        m_selectedPortraitButton.onClick.AddListener(OnPortraitSelected);
        WorldSelection.OnSelectionChanged += OnSelectionChanged;
    }

    private void OnDisable()
    {
        m_selectedPortraitButton.onClick.RemoveListener(OnPortraitSelected);
        WorldSelection.OnSelectionChanged -= OnSelectionChanged;
    }

    private void OnPortraitSelected()
    {
        var selectedObject = WorldSelection.SelectedObject as MonoBehaviour;
        m_cameraController.SetCameraPosition(selectedObject.transform.position);
    }

    private void OnSelectionChanged(object sender, WorldSelection.SelectionChangedData selection)
    {
        m_selectedUnitHolder.SetActive(selection.Current is Unit);
        m_selectedPortraitHolder.SetActive(selection.Current is ITileObject);
        m_selectedRuinHolder.SetActive(selection.Current is Ruin);

        if (selection.Current is Unit unit)
        {
            m_unitHealthText.text = unit.GetHealth().ToString() + "/" + UnitFactory.Instance.GetBaseUnitStats(unit.Type).health;
            m_unitMovementText.text = unit.Movement.ToString() + "/" + unit.GetMovementSpeed().ToString();
            m_unitSightText.text = unit.GetSight().ToString();
            m_unitDamageText.text = unit.GetDamage().ToString();
            m_unitNameText.text = unit.GetCurrentUnitType().ToString();

            m_portraitImage.sprite = unit.Sprite;

        }
        else if (selection.Current is Ruin ruin)
        {
            m_portraitImage.sprite = ruin.Sprite;
        }
    }

    private void ChangeRuinUnitType(Unit.EUnitType type)
    {
        if (WorldSelection.SelectedObject is Ruin ruin)
        {
            ruin.UnitType = type;

            foreach (RuinSelectAction action in m_unitTypeActions)
            {
                action.SelectedImage.enabled = action.UnitType == type;
                action.SelectButton.interactable = action.UnitType != type;
                FindObjectOfType<AudioController>().PlayAudio(AudioType.SFX_02, true);
            }

            XMLFormatter.AddUnitTypeChange(ruin, type);
        }
    }
}