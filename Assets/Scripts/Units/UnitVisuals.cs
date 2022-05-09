using TMPro;
using UnityEngine;

public class UnitVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer unitSprite;
    [SerializeField] private GameObject unitText;
    [SerializeField] private int flicksOnDamage = 5;
    [SerializeField] private float timePerFlick = 0.2f;
    [SerializeField] private float floatTime = 1.5f;
    [SerializeField] private float floatHeight = 1.0f;

    private float textDamageTimer = 0;

    private float damageTimer = 0.0f;
    private int flicks = 0;
    private bool specialVisuals = false;
    private bool color = false;


    // Update is called once per frame
    private void Update()
    {
        if (!specialVisuals)
        {
            return;
        }

        if (textDamageTimer > 0)
        {
            textDamageTimer -= Time.deltaTime;
            unitText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, ((floatTime - textDamageTimer) / floatTime) * floatHeight);
        }

        if (damageTimer > 0)
        {
            damageTimer -= Time.deltaTime;
        }
        else if (flicks > 0)
        {
            flicks--;
            damageTimer = timePerFlick;
            unitSprite.color = color ? new Color(1, 1, 1) : new Color(1, 0, 0);
            color = !color;
        }
        else if (color)
        {
            unitSprite.color = new Color(1, 1, 1);
            color = false;
        }
        else if (textDamageTimer <= 0)
        {
            specialVisuals = false;
            unitText.SetActive(false);
        }
    }

    public void TookDamage(int dmg)
    {
        unitSprite.color = new Color(1, 0, 0);
        unitText.SetActive(true);
        unitText.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        unitText.GetComponent<TextMeshPro>().text = "-" + dmg.ToString();
        specialVisuals = true;
        color = true;
        flicks = flicksOnDamage;
        damageTimer = timePerFlick;
        textDamageTimer = floatTime;
    }
}
