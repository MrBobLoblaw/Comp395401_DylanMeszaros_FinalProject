using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class TestTube : MonoBehaviour
{
    public ToolData tool;

    public int maxNumberOfSubstances = 2;
    public float timeToMix = 3.0f;
    public List<Sprite> images;
    public string solutionName;

    private bool full = false;
    private bool mixing = false;
    private bool failed = false;
    private float timeElapsed = 0f;
    private Color latestSubstanceColor = Color.white;
    /*private List<ElementData> elements = new List<ElementData>();
    private List<CompoundData> compounds = new List<CompoundData>();
    private List<SolutionData> solutions = new List<SolutionData>();*/
    private List<string> substances = new List<string>();

    public bool IsFull() { return full; }
    public bool IsMixing() { return mixing; }

    public Color GetLatestColor() { return latestSubstanceColor; }

    public List<string> GetSubstances() { return substances; }

    private void Start()
    {
        DisplayTube();
    }

    private void Update()
    {
        if (mixing)
        {
            timeElapsed += Time.deltaTime;

            TextMeshProUGUI solutionTitle = GameObjectFinder.FindChildRecursive(gameObject, "SolutionTitle").GetComponent<TextMeshProUGUI>();
            solutionTitle.text = "Mixing";
            if (timeElapsed / timeToMix > 0.75f) { solutionTitle.text += "..."; }
            else if (timeElapsed / timeToMix > 0.5f) { solutionTitle.text += ".."; }
            else if (timeElapsed / timeToMix > 0.25f) { solutionTitle.text += "."; }

            if (timeElapsed >= timeToMix)
            {
                mixing = false;
                timeElapsed = 0f;

                MixSubstances();
            }
        }
    }

    private void DisplayTube()
    {
        //List<string> substanceNames = new List<string>();
        string tubeName = "Empty";

        Color tubeColor = latestSubstanceColor;

        /*foreach (ElementData element in elements)
        {
            substanceNames.Add(element.name);
            tubeName = "";
            tubeColor = element.colorKey;
        }

        foreach (CompoundData compound in compounds)
        {
            substanceNames.Add(compound.name);
            tubeName = "";
            tubeColor = compound.colorKey;
        }

        foreach (SolutionData solution in solutions)
        {
            substanceNames.Add(solution.name);
            tubeName = "";
            tubeColor = solution.colorKey;
        }*/

        if (substances.Count > 0) { tubeName = ""; }

        foreach (string substanceName in substances)//substanceNames)
        {
            if (tubeName != "" ) { tubeName += " + "; }
            tubeName += substanceName;
        }

        Image tubeImage = GetComponent<Image>();
        //if (elements.Count + compounds.Count + solutions.Count == 0) { tubeImage.sprite = images[0]; } else { tubeImage.sprite = images[1]; }
        if (substances.Count == 0) { tubeImage.sprite = images[0]; } else { tubeImage.sprite = images[1]; }
        tubeImage.color = tubeColor;

        TextMeshProUGUI solutionTitle = GameObjectFinder.FindChildRecursive(gameObject, "SolutionTitle").GetComponent<TextMeshProUGUI>();
        solutionTitle.text = tubeName;



        solutionName = tubeName;

        if (failed) { solutionTitle.text = "Failed Mixture"; tubeImage.color = Color.black; }


    }

    private void MixSubstances()
    {
        bool mixtureFound = false;

        foreach (Mixture mixture in tool.mixtures)
        {
            if (mixture.substances.Contains(substances[0]) && mixture.substances.Contains(substances[1]))
            {
                mixtureFound = true;

                Clean();

                substances.Add(mixture.result.name);
                latestSubstanceColor = mixture.result.colorKey;
            }
        }

        if (!mixtureFound)
        {
            Debug.Log("Mixture Failed");

            failed = true;

            return;
        }
        else
        {
            Debug.Log("Mixture Succedded");
        }

        DisplayTube();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (full || mixing || failed) return;

        // If Another Element is Hit
        if (collision.gameObject.CompareTag("Element"))
        {

            // Verify Element Exists
            Element other = collision.gameObject.GetComponent<Element>();
            if (other != null)
            {

                // Debug Collision
                //Debug.Log($"{data.name} collided with {other.data.name}");

                if (other.data.getToolType().Contains("Tube"))
                {
                    bool canAdd = false;

                    // Add Element
                    foreach (Mixture mixture in tool.mixtures)
                    {
                        if (!mixture.substances.Contains(other.data.name)) { continue; } // ignore this mixture if element isn't relevant.

                        if (substances.Count > 0)
                        {
                            if (!mixture.substances.Contains(substances[0])) { continue; }
                            //if (substances[1] != null) { if (!mixture.substances.Contains(substances[1])) { continue; } } // 3 max rn
                        }

                        if (!substances.Contains(other.data.name)) { canAdd = true; break; }
                        //foreach (ElementData element in elements) { if (element.name == other.data.name) { continue; } }
                    }

                    //if (elements.Count + compounds.Count + solutions.Count == 0) { canAdd = true; } // If the tube is empty, allow it anyways.
                    if (substances.Count == 0) { canAdd = true; }

                    if (canAdd)
                    {
                        substances.Add(other.data.name);

                        //if (elements.Count + compounds.Count + solutions.Count == maxNumberOfSubstances) { full = true; }
                        if (substances.Count == maxNumberOfSubstances) { full = true; mixing = true; }

                        latestSubstanceColor = other.data.colorKey;

                        DisplayTube();

                        // Remove Element Source
                        Destroy(other.gameObject);
                    }
                }
            }
        }

        // If Another Compound is Hit
        if (collision.gameObject.CompareTag("Compound"))
        {

            // Verify Compound Exists
            Compound other = collision.gameObject.GetComponent<Compound>();
            if (other != null)
            {

                // Debug Collision
                //Debug.Log($"{data.name} collided with {other.data.name}");

                if (other.data.getToolType().Contains("Tube"))
                {
                    bool canAdd = false;

                    // Add Element
                    foreach (Mixture mixture in tool.mixtures)
                    {
                        if (!mixture.substances.Contains(other.data.name)) { continue; } // ignore this mixture if element isn't relevant.

                        if (substances.Count > 0)
                        {
                            if (!mixture.substances.Contains(substances[0])) { continue; }
                            //if (substances[1] != null) { if (!mixture.substances.Contains(substances[1])) { continue; } } // 3 max rn
                        }

                        if (!substances.Contains(other.data.name)) { canAdd = true; break; }
                        //foreach (ElementData element in elements) { if (element.name == other.data.name) { continue; } }
                    }

                    //if (elements.Count + compounds.Count + solutions.Count == 0) { canAdd = true; } // If the tube is empty, allow it anyways.
                    if (substances.Count == 0) { canAdd = true; }

                    if (canAdd)
                    {
                        substances.Add(other.data.name);

                        //if (elements.Count + compounds.Count + solutions.Count == maxNumberOfSubstances) { full = true; }
                        if (substances.Count == maxNumberOfSubstances) { full = true; mixing = true; }

                        latestSubstanceColor = other.data.colorKey;

                        DisplayTube();

                        // Remove Element Source
                        Destroy(other.gameObject);
                    }
                }
            }
        }

        // If Another Compound is Hit
        if (collision.gameObject.CompareTag("Tube"))
        {

            // Verify Compound Exists
            TestTube other = collision.gameObject.GetComponent<TestTube>();
            if (other != null)
            {

                if (other.GetSubstances().Count == 0 || other.GetSubstances().Count > 1) { return; } // only dealing with 1 substance

                bool canAdd = false;

                // Add Element
                foreach (Mixture mixture in tool.mixtures)
                {
                    if (!mixture.substances.Contains(other.GetSubstances()[0])) { continue; } // ignore this mixture if element isn't relevant.

                    if (substances.Count > 0)
                    {
                        if (!mixture.substances.Contains(substances[0])) { continue; }
                        //if (substances[1] != null) { if (!mixture.substances.Contains(substances[1])) { continue; } } // 3 max rn
                    }

                    if (!substances.Contains(other.GetSubstances()[0])) { canAdd = true; break; }
                    //foreach (ElementData element in elements) { if (element.name == other.data.name) { continue; } }
                }

                //if (elements.Count + compounds.Count + solutions.Count == 0) { canAdd = true; } // If the tube is empty, allow it anyways.
                if (substances.Count == 0) { canAdd = true; } // If this tube is empty, allow it anyways.

                if (canAdd)
                {
                    substances.Add(other.GetSubstances()[0]);

                    //if (elements.Count + compounds.Count + solutions.Count == maxNumberOfSubstances) { full = true; mixing = true; }
                    if (substances.Count == maxNumberOfSubstances) { full = true; mixing = true; }

                    latestSubstanceColor = other.GetLatestColor();

                    DisplayTube();

                    // Remove Element Source
                    other.Clean();
                }
            }
        }
    }

    public void Clean()
    {
        full = false;
        mixing = false;
        timeElapsed = 0f;
        //elements.Clear();
        //compounds.Clear();
        //solutions.Clear();
        substances.Clear();
        DisplayTube();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Dropped in cleaner
        if (collision.gameObject.CompareTag("Cleaner"))
        {
            Clean();
        }
    }
}
