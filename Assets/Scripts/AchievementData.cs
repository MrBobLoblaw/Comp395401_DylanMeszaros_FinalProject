using System.Collections.Generic;
using UnityEngine;

public enum MeasurementType
{
    Units,
    SecondsRemaining,
    Level
}

public enum DifficultyTier
{
    Bronze,
    Silver,
    Gold
}

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievement")]
public class AchievementData : ScriptableObject
{
    public string description;
    public Sprite icon;
    public DifficultyTier difficultyTier;

    public MeasurementType measurementType;
    public int goalValue;
}
