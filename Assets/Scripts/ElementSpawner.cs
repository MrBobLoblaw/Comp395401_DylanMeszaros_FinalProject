using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    [Header("Naviagtion")]
    public GameObject workspace;

    [Header("Element Properties")]
    public Element elementPrefab;

    public ElementData elementData;

    public void SpawnElement()
    {
        Element newElement = Instantiate(elementPrefab, transform.position, Quaternion.identity);
        newElement.data = elementData;

        //newElement.transform.parent = workspace.transform;
        newElement.transform.SetParent(workspace.transform, true);
    }
}
