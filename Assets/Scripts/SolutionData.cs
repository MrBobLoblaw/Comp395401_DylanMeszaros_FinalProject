using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Dependencies.Sqlite.SQLite3;

[CreateAssetMenu(fileName = "New Solution", menuName = "Solution")]
public class SolutionData : ScriptableObject
{
    [EnumFlagsAttribute] public ToolType toolType;

    public string solutionSymbol;
    public Sprite icon;
    public Color colorKey = Color.white;

    public List<string> getToolType()
    {
        return EnumFlagsAttribute.GetSelectedStrings(toolType);
    }
}
