using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum SubmissionType
{
    Compound,
    TubeSolution,
    Beaker
}

public enum AnswerType
{
    AND,
    OR
}

[System.Serializable]
public class UpdateAchievement
{
    public MeasurementType measurementType;
    public string achievementName;
    public int amount;

    public void AdvanceUpdate()
    {
        GameManager gameManager = GameManager.Instance;

        gameManager.AdvanceAchievementBy(achievementName, amount);
    }
}

public class AnswerZone : MonoBehaviour
{
    public SubmissionType submissionType;
    public AnswerType answerType;
    private List<string> submittedAnswers = new List<string>();
    public List<string> requiredAnswers;

    private bool submitted = false;
    public Button nextLevelbutton;

    public List<UpdateAchievement> updateAchievements;

    private float timeElapsed;
    public GameObject timerTextbox;

    private void Start()
    {

        if (nextLevelbutton == null)
        {
            Button searchFound = GameObjectFinder.FindChildRecursive(CanvasHandler.Instance.gameObject, "NextLevelButton").GetComponent<Button>();

            if (searchFound != null)
            {
                nextLevelbutton = searchFound;

                nextLevelbutton.interactable = false;
            }
        }
        else
        {
            nextLevelbutton.interactable = false;
        }
    }

    private void Update()
    {
        if (timerTextbox != null)
        {
            if (!submitted)
            {
                timeElapsed += Time.deltaTime;
                timerTextbox.GetComponent<TextMeshProUGUI>().text = $"{(Mathf.Round(timeElapsed * 10.0f) * 0.1f)}s";
            }
            else
            {
                //timerTextbox.GetComponent<TextMeshProUGUI>().text = $"{(Mathf.Round(timeElapsed * 10.0f) * 0.1f)}s";
                timerTextbox.GetComponent<TextMeshProUGUI>().color = Color.green;

            }
        }
    }

    private bool CheckRequirements()
    {
        if (answerType == AnswerType.OR)
        {
            foreach (string requirement in requiredAnswers)
            {
                foreach (string submission in submittedAnswers)
                {
                    if (submission.Equals(requirement))
                    {
                        foreach (UpdateAchievement updateAchievement in updateAchievements)
                        {
                            if (updateAchievement.measurementType == MeasurementType.SecondsRemaining) { updateAchievement.amount = (int)timeElapsed; }

                            updateAchievement.AdvanceUpdate();
                        }

                        return true;
                    }
                }
            }
        }

        bool requirementsFulfilled = requiredAnswers.All(answer => submittedAnswers.Contains(answer));

        if (requirementsFulfilled)
        {
            foreach (UpdateAchievement updateAchievement in updateAchievements)
            {
                if (updateAchievement.measurementType == MeasurementType.SecondsRemaining)
                {
                    if ((int)timeElapsed < GameManager.Instance.GetAchievementProgress(updateAchievement.achievementName))
                    {
                        updateAchievement.amount = (int)timeElapsed;
                    }
                }

                updateAchievement.AdvanceUpdate();
            }
        }

        return requirementsFulfilled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (submissionType)
        {
            case SubmissionType.Compound:
                {
                    // If Another Compound is Hit
                    if(other.gameObject.CompareTag("Compound"))
                    {

                        // Verify Compound Exists
                        Compound compound = other.gameObject.GetComponent<Compound>();
                        if (compound != null)
                        {

                            bool submissionMatches = false;
                            foreach (string requirement in requiredAnswers) { if (requirement == compound.name) { submissionMatches = true; } }
                            if (!submissionMatches) { return; }

                            submittedAnswers.Add(compound.name);

                            other.gameObject.GetComponent<Draggable>().enabled = false;
                            compound.enabled = false;

                            if (CheckRequirements())
                            {
                                nextLevelbutton.interactable = true;

                                submitted = true;
                            }

                        }
                    }
                    break;
                }
            case SubmissionType.TubeSolution:
                {
                    // If Another Compound is Hit
                    Debug.Log("AnswerZone: Tube Solution");
                    if (other.gameObject.CompareTag("Tube"))
                    {
                        Debug.Log("AnswerZone: Tube Hit");
                        // Verify Compound Exists
                        TestTube tube = other.gameObject.GetComponent<TestTube>();
                        if (tube != null)
                        {
                            Debug.Log("AnswerZone: Tube Exists");
                            bool submissionMatches = false;
                            string solutionName = GameObjectFinder.FindChildRecursive(tube.gameObject, "SolutionTitle").GetComponent<TextMeshProUGUI>().text;
                            foreach (string requirement in requiredAnswers) { if (requirement == solutionName) { submissionMatches = true; } }
                            if (!submissionMatches) { return; }

                            submittedAnswers.Add(solutionName);

                            other.gameObject.GetComponent<Draggable>().enabled = false;
                            tube.enabled = false;

                            if (CheckRequirements())
                            {
                                nextLevelbutton.interactable = true;

                                submitted = true;
                            }

                        }
                    }
                    break;
                }
            case SubmissionType.Beaker:
                {
                    // If Another Compound is Hit
                    if (other.gameObject.CompareTag("Beaker"))
                    {

                        // Verify Compound Exists
                        Compound compound = other.gameObject.GetComponent<Compound>();
                        if (compound != null)
                        {

                            bool submissionMatches = false;
                            foreach (string requirement in requiredAnswers) { if (requirement == compound.name) { submissionMatches = true; } }
                            if (!submissionMatches) { return; }

                            submittedAnswers.Add(compound.name);

                            other.gameObject.GetComponent<Draggable>().enabled = false;
                            compound.enabled = false;

                            if (CheckRequirements())
                            {
                                nextLevelbutton.interactable = true;

                                submitted = true;
                            }

                        }
                    }
                    break;
                }
        }
    }

    public void NextLevel(string levelName)
    {
        if (!submitted)
        {
            if (ReactionLogger.Instance) { ReactionLogger.Instance.LogReaction("Missing Requirements"); }

            return;
        }

        // Load the next scene (Lab 2)
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }
}
