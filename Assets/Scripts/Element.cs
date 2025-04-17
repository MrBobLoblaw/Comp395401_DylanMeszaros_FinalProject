using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class Element : MonoBehaviour
{
    public ElementData data;

    private void Start()
    {
        if (data != null)
        {
            //GetComponent<SpriteRenderer>().sprite = data.icon;
            //GetComponent<SpriteRenderer>().color = data.colorKey;
            gameObject.name = data.name;

            DisplayElement();
        }
    }

    private void DisplayElement()
    {
        Image elementImage = GameObjectFinder.FindChildRecursive(gameObject, "Icon").GetComponent<Image>();
        TextMeshProUGUI symbolText = GameObjectFinder.FindChildRecursive(gameObject, "SymbolText").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI nameText = GameObjectFinder.FindChildRecursive(gameObject, "NameText").GetComponent<TextMeshProUGUI>();

        elementImage.sprite = data.icon;
        symbolText.text = data.elementSymbol;
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
        
        // If Another Element is Hit
        if (collision.gameObject.CompareTag("Element"))
        {

            // Verify Element Exists
            Element other = collision.gameObject.GetComponent<Element>();
            if (other != null)
            {

                // Debug Collision
                Debug.Log($"{data.name} collided with {other.data.name}");

                foreach (Interaction interaction in data.interactions)
                {
                    if (interaction.interactedElement == other.data)
                    {
                        Debug.Log("Run Reaction");
                        Reaction reaction = Instantiate(interaction.reaction, transform.position, Quaternion.identity).GetComponent<Reaction>();
                        reaction.transform.parent = transform.parent;
                        reaction.compoundData = interaction.compoundData;
                        reaction.causeOrigins.Add(gameObject);
                        reaction.causeOrigins.Add(collision.gameObject);
                        reaction.Effect();
                    }
                }
            }
        }
    }
}
