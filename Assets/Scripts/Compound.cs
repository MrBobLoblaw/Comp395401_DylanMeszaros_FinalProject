using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Compound : MonoBehaviour
{
    public CompoundData data;

    private void Start()
    {
        if (data != null)
        {
            //GetComponent<SpriteRenderer>().sprite = data.icon;
            //GetComponent<SpriteRenderer>().color = data.colorKey;
            gameObject.name = data.name;

            DisplayCompound();
        }
    }

    private void DisplayCompound()
    {
        Image compoundImage = GameObjectFinder.FindChildRecursive(gameObject, "Icon").GetComponent<Image>();
        TextMeshProUGUI symbolText = GameObjectFinder.FindChildRecursive(gameObject, "SymbolText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = GameObjectFinder.FindChildRecursive(gameObject, "NameText").GetComponent<TextMeshProUGUI>();

        compoundImage.sprite = data.icon;
        symbolText.text = data.compoundSymbol;
        nameText.text = data.name;

        GameObject accessibleToolsTube = GameObjectFinder.FindChildRecursive(gameObject, "Tube");
        GameObject accessibleToolsBeaker = GameObjectFinder.FindChildRecursive(gameObject, "Beaker");
        GameObject tubeBan = GameObjectFinder.FindChildRecursive(accessibleToolsTube, "Banned");
        GameObject beakerBan = GameObjectFinder.FindChildRecursive(accessibleToolsBeaker, "Banned");

        if (!data.getToolType().Contains("Tube")) { tubeBan.SetActive(true); }
        if (!data.getToolType().Contains("Beaker")) { beakerBan.SetActive(true); }

        gameObject.GetComponent<Image>().color = data.colorKey;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        
    }
}
