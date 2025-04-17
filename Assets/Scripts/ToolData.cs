using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;

[System.Serializable]
public class Mixture
{
    [Header("Mixture")]
    public List<string> substances = new List<string>();
    public SolutionData result;
    public bool positiveReaction = true;
}

[CreateAssetMenu(fileName = "New Tool", menuName = "Tool")]
public class ToolData : ScriptableObject
{
    public ToolType toolType;

    public List<Mixture> mixtures;
}
