using TMPro;
using UnityEngine;

public class ReactionLogger : MonoBehaviour
{
    public TextMeshProUGUI logText;

    public static ReactionLogger Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GetComponent<TextMeshProUGUI>() != null)
        {
            logText = GetComponent<TextMeshProUGUI>();
        }
    }

    public void LogReaction(string message)
    {
        logText.text += message + "\n";
    }
}
