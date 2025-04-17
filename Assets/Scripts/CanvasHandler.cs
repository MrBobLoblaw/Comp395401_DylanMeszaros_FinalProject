using UnityEngine;

public class CanvasHandler : MonoBehaviour
{
    public static CanvasHandler Instance;

    private void Awake()
    {
        Instance = this;
    }
}
