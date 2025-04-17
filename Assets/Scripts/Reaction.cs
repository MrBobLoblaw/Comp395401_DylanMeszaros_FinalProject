using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Reaction : MonoBehaviour
{
    public GameObject compoundPrefab;

    [Header("Reaction Properties")]
    public List<GameObject> causeOrigins;
    public CompoundData compoundData;

    public virtual void Effect()
    {
        if (compoundData != null)
        {
            // Create Compound
            GameObject compoundObject = Instantiate(compoundPrefab, transform.position, Quaternion.identity);
            compoundObject.GetComponent<Compound>().data = compoundData;
            //compoundObject.transform.parent = transform.parent;
            compoundObject.transform.SetParent(transform.parent, true);
            if (ReactionLogger.Instance) { ReactionLogger.Instance.LogReaction($"[Reaction] [{causeOrigins[0].name} & {causeOrigins[1].name}] [Success] [Result: {compoundData.name}]"); }
        }
        else
        {
            // Explosion
            if (ReactionLogger.Instance) { ReactionLogger.Instance.LogReaction($"[Reaction] [{causeOrigins[0].name} & {causeOrigins[1].name}] [Failed]"); }
        }

        foreach (GameObject origin in causeOrigins)
        {
            Destroy(origin);
        }
        Destroy(gameObject);
        return;
    }
}
