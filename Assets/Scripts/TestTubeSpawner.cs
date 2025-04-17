using UnityEngine;

public class TestTubeSpawner : MonoBehaviour
{
    [Header("Naviagtion")]
    public GameObject workspace;

    [Header("Element Properties")]
    public TestTube tubePrefab;

    public void SpawnTestTube()
    {
        TestTube newTube = Instantiate(tubePrefab, transform.position, Quaternion.identity);

        newTube.transform.parent = workspace.transform;
    }
}
