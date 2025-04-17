using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

[CreateAssetMenu(fileName = "New Compound", menuName = "Compound")]
public class CompoundData : ScriptableObject
{
    [EnumFlagsAttribute] public ToolType toolType;

    public string compoundSymbol; // Example: H2O, CO2, CH4
    public Sprite icon;
    public Color colorKey = Color.white;

    public List<string> getToolType()
    {
        return EnumFlagsAttribute.GetSelectedStrings(toolType);
    }
}
