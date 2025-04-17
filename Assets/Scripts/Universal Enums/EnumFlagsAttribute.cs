using System.Collections.Generic;
using System;
using UnityEngine;

public class EnumFlagsAttribute : PropertyAttribute
{
    public EnumFlagsAttribute() { }

    public static List<int> GetSelectedIndexes<T>(T val) where T : IConvertible
    {
        List<int> selectedIndexes = new List<int>();
        for (int i = 0; i < System.Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedIndexes.Add(i);
            }
        }
        return selectedIndexes;
    }
    public static List<string> GetSelectedStrings<T>(T val) where T : IConvertible
    {
        List<string> selectedStrings = new List<string>();
        for (int i = 0; i < Enum.GetValues(typeof(T)).Length; i++)
        {
            int layer = 1 << i;
            if ((Convert.ToInt32(val) & layer) != 0)
            {
                selectedStrings.Add(Enum.GetValues(typeof(T)).GetValue(i).ToString());
            }
        }
        return selectedStrings;
    }
}
