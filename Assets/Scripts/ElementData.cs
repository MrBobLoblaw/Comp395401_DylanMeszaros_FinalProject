using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.InputSystem.Editor.InputActionCodeGenerator;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "New Element", menuName = "Element")]
public class ElementData : ScriptableObject
{
    [EnumFlagsAttribute] public ToolType toolType;

    public string elementSymbol; // Example: H, Na, Sc, Ni, Al, F
    public Sprite icon;
    public Color colorKey = Color.white;
    public List<Interaction> interactions;

    public List<string> getToolType()
    {
        return EnumFlagsAttribute.GetSelectedStrings(toolType);
    }
}
