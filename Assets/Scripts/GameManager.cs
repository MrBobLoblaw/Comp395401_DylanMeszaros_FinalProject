using NUnit.Framework;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

[System.Serializable]
public class Achievement
{
    public AchievementData data;
    public int progress;

    public Achievement(AchievementData data)
    {
        this.data = data;

        if (data.measurementType == MeasurementType.SecondsRemaining)
        {
            progress = data.goalValue * 4;
        }
    }

    public bool IsCompleted()
    {
        if (data.measurementType == MeasurementType.SecondsRemaining)
        {
            return progress <= data.goalValue;
        }
        else
        {
            return progress / data.goalValue >= 1f;
        }
    }

    public void ChangeProgressBy(int amount)
    {
        if (data.measurementType == MeasurementType.Units)
        {
            progress += amount;
        }
        else if(data.measurementType == MeasurementType.SecondsRemaining)
        {
            progress = amount;
        }
        else if (data.measurementType == MeasurementType.Level)
        {
            progress = 1;
        }
    }
}


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject gameStartMenu;

    public GameObject achievementPrefab;
    public GameObject achievementsMenu;

    public List<AchievementData> gameAchievementsData = new List<AchievementData>();

    private List<Achievement> achievements = new List<Achievement>();
    private List<GameObject> achievementCards = new List<GameObject>();

    private bool isLevelsMenuOpen = false;
    private bool isAchievementsMenuOpen = false;

    [Header("Keybinds")]
    private Scene lastScene;
    public KeyCode returnKey = KeyCode.Escape;

    void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;

            // Start the game by creating all the achievements
            foreach (AchievementData achievementData in gameAchievementsData)
            {
                achievements.Add(new Achievement(achievementData));
            }

            SearchForFeatures();

            CreateAchievementCards();

            DontDestroyOnLoad(gameObject); // Keep across scenes

            // Subscribe to scene load events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (Instance != this)
        {
            SearchForFeatures();

            LoadAchievementDisplays();

            Destroy(gameObject); // Prevent duplicates
        }
    }

    private void Start()
    {
        /*SearchForFeatures();

        if (gameStartMenu != null)
        {
            if (IsAchievementCompleted("Tutorial"))
            {
                Debug.Log("Load Levels Menu");

                GameObject startGameButton = GameObjectFinder.FindChildRecursive(gameStartMenu, "StartButton");
                GameObject levelsButton = GameObjectFinder.FindChildRecursive(gameStartMenu, "LevelsButton");

                startGameButton.SetActive(false);
                levelsButton.SetActive(true);
            } 
        }*/
    }

    private void Update()
    {
        if (Input.GetKey(returnKey))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private void SearchForFeatures()
    {
        if (gameStartMenu == null)
        {
            GameObject searchedObject = GameObjectFinder.FindChildRecursive(CanvasHandler.Instance.gameObject, "GameStart");

            if (searchedObject != null) // If Achievement Menu was found in a new scene
            {
                gameStartMenu = searchedObject;
            }
        }

        if (achievementsMenu == null) // If not preset, or moved scenes and no longer available
        {
            GameObject searchedObject = GameObjectFinder.FindChildRecursive(CanvasHandler.Instance.gameObject, "AchievementsMenu");

            if (searchedObject != null) // If Achievement Menu was found in a new scene
            {
                achievementsMenu = searchedObject;

                GameObject achievementsButton = GameObjectFinder.FindChildRecursive(achievementsMenu, "AchievementsButton");

                achievementsButton.GetComponent<Button>().onClick.AddListener(ToggleAchievementsMenu);
            }

            CreateAchievementCards();
        }
    }

    public void ToggleLevelMenu()
    {
        isLevelsMenuOpen = !isLevelsMenuOpen;



        GameObject levelMenu = GameObjectFinder.FindChildRecursive(CanvasHandler.Instance.gameObject, "LevelScreen");

        Debug.Log(levelMenu);

        if (isLevelsMenuOpen)
        {
            levelMenu.SetActive(true);

            GameObject labButtons = GameObjectFinder.FindChildRecursive(levelMenu, "TestTubeButtons");

            GameObject lab1Button = GameObjectFinder.FindChildRecursive(labButtons, "Lab1Button");
            if (IsAchievementCompleted("Tutorial")) { lab1Button.GetComponent<Button>().interactable = true; }

            GameObject lab2Button = GameObjectFinder.FindChildRecursive(labButtons, "Lab2Button");
            if (IsAchievementCompleted("TestTubeLab1")) { lab2Button.GetComponent<Button>().interactable = true; }

            GameObject lab3Button = GameObjectFinder.FindChildRecursive(labButtons, "Lab3Button");
            if (IsAchievementCompleted("TestTubeLab2")) { lab3Button.GetComponent<Button>().interactable = true; }

            GameObject lab4Button = GameObjectFinder.FindChildRecursive(labButtons, "Lab4Button");
            if (IsAchievementCompleted("TestTubeLab3")) { lab4Button.GetComponent<Button>().interactable = true; }

            GameObject lab5Button = GameObjectFinder.FindChildRecursive(labButtons, "Lab5Button");
            if (IsAchievementCompleted("TestTubeLab4")) { lab5Button.GetComponent<Button>().interactable = true; }

            GameObject sandboxButton = GameObjectFinder.FindChildRecursive(labButtons, "SandboxButton");
            if (IsAchievementCompleted("TestTubeLab5")) { sandboxButton.GetComponent<Button>().interactable = true; }
        }
        else
        {
            levelMenu.SetActive(false);
        }
    }

    public void ToggleAchievementsMenu()
    {
        if (achievementsMenu == null) { return; }

        isAchievementsMenuOpen = !isAchievementsMenuOpen;

        TextMeshProUGUI menuButton = GameObjectFinder.FindChildRecursive(achievementsMenu, "MenuButtonText").GetComponent<TextMeshProUGUI>();
        GameObject achievementList = GameObjectFinder.FindChildRecursive(achievementsMenu, "AchievementList");

        if (isAchievementsMenuOpen) // open
        {
            menuButton.text = "Close";

            achievementList.SetActive(true);

            CreateAchievementCards();

            GameObject notification = GameObjectFinder.FindChildRecursive(achievementsMenu, "Notification");
            notification.SetActive(false);
        }
        else // not open
        {
            menuButton.text = "Achievements";

            achievementList.SetActive(false);
        }
    }

    public void AdvanceAchievementBy(string achievementName, int amount)
    {
        foreach (Achievement achievement in achievements)
        {
            bool hasCompleted = achievement.IsCompleted();

            if (achievement.data.name == achievementName)
            {
                achievement.ChangeProgressBy(amount);

                if (!hasCompleted)
                {
                    hasCompleted = achievement.IsCompleted();

                    if (hasCompleted)
                    {
                        if (!isAchievementsMenuOpen && achievementsMenu != null)
                        {
                            GameObject notification = GameObjectFinder.FindChildRecursive(achievementsMenu, "Notification");
                            notification.SetActive(true);
                        }
                    }
                }

                int totalCompletedAchievements = 0;
                Achievement totalityAchievement = null;

                foreach (Achievement checkAchievement in achievements)
                {
                    if (IsAchievementCompleted(checkAchievement.data.name)) { totalCompletedAchievements += 1; }
                    if (checkAchievement.data.name == "Totality") { totalityAchievement = checkAchievement; }
                }

                if (totalityAchievement != null)
                {
                    if (totalCompletedAchievements > totalityAchievement.data.goalValue) { totalCompletedAchievements = totalityAchievement.data.goalValue; }
                    totalityAchievement.progress = totalCompletedAchievements;
                }
            }
        }
    }

    public bool IsAchievementCompleted(string achievementName)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.data.name == achievementName)
            {
                return achievement.IsCompleted();
            }
        }

        return false;
    }

    public int GetAchievementProgress(string achievementName)
    {
        foreach (Achievement achievement in achievements)
        {
            if (achievement.data.name == achievementName)
            {
                return achievement.progress;
            }
        }

        return 0;
    }

    private void CreateAchievementCards()
    {
        DeleteAchievementCards();

        if (achievementCards.Count == 0)
        {
            foreach (Achievement achievement in achievements)
            {
                GameObject newAchievementCard = Instantiate(achievementPrefab);
                achievementCards.Add(newAchievementCard);

                //newAchievementCard.transform.parent = GameObjectFinder.FindChildRecursive(achievementsMenu, "Content").transform;
                newAchievementCard.transform.SetParent(GameObjectFinder.FindChildRecursive(achievementsMenu, "Content").transform, false);
                newAchievementCard.GetComponent<RectTransform>().localScale = new Vector3(0.9f, 0.9f, 1f);
            }
        }

        LoadAchievementDisplays();
    }

    private void LoadAchievementDisplays()
    {
        if (achievementCards.Count == 0) { return; } // Can't display achievements if they haven't been visibly made yet.

        Debug.Log("Load AchievementCard Display");

        for (int i = 0; i < achievements.Count; i++)
        {
            Achievement achievement = achievements[i];
            GameObject achievementCard = achievementCards[i];
            Image background = GameObjectFinder.FindChildRecursive(achievementCard, "Background").GetComponent<Image>();
            
            // Difficulty Color
            if (achievement.data.difficultyTier == DifficultyTier.Bronze)
            {
                achievementCard.GetComponent<Image>().color = new Color(214f / 255f, 104f / 255f, 10f / 255f); // rich bronze
                background.color = new Color(219f / 255f, 181f / 255f, 99f / 255f); // pale bronze
            }
            else if (achievement.data.difficultyTier == DifficultyTier.Silver)
            {
                achievementCard.GetComponent<Image>().color = new Color(191f / 255f, 226f / 255f, 240f / 255f); // rich silver
                background.color = new Color(220f / 255f, 234f / 255f, 240f / 255f); // pale silver
            }
            else if (achievement.data.difficultyTier == DifficultyTier.Gold)
            {
                achievementCard.GetComponent<Image>().color = new Color(240f / 255f, 204f / 255f, 25f / 255f); // rich gold
                background.color = new Color(240f / 255f, 226f / 255f, 126f / 255f); // pale gold
            }

            // Icon
            Image icon = GameObjectFinder.FindChildRecursive(achievementCard, "Icon").GetComponent<Image>();
            icon.sprite = achievement.data.icon;

            // Description
            TextMeshProUGUI description = GameObjectFinder.FindChildRecursive(achievementCard, "Description").GetComponent<TextMeshProUGUI>();
            description.text = achievement.data.description;

            // Progress Bar
            GameObject progressBar = GameObjectFinder.FindChildRecursive(achievementCard, "ProgressBar");
            GameObject progressText = GameObjectFinder.FindChildRecursive(achievementCard, "ProgressText");
            if (achievement.progress >= achievement.data.goalValue)
            {
                progressBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    0f, 
                    progressBar.GetComponent<RectTransform>().anchoredPosition.y
                );
                //int absoluteProgress = achievement.progress; //if (absoluteProgress > achievement.data.goalValue) { absoluteProgress = achievement.data.goalValue; }
                progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    0f, 
                    progressBar.GetComponent<RectTransform>().sizeDelta.y
                );
                if (achievement.data.measurementType == MeasurementType.Units || achievement.data.measurementType == MeasurementType.Level)
                {
                    progressText.GetComponent<TextMeshProUGUI>().text = "Completed";
                }
                else if (achievement.data.measurementType == MeasurementType.SecondsRemaining)
                {
                    if (achievement.progress == achievement.data.goalValue)
                    {
                        progressText.GetComponent<TextMeshProUGUI>().text = $"Completed in {achievement.data.goalValue} Seconds";
                        progressBar.GetComponent<Image>().color = new Color(121f / 255f, 255f / 255f, 122f / 255f); // green
                    }
                    else
                    {
                        progressText.GetComponent<TextMeshProUGUI>().text = $"{achievement.progress} / {achievement.data.goalValue} Seconds left";
                        progressBar.GetComponent<Image>().color = new Color(255f / 255f, 174f / 255f, 0.1f / 255f); // orange
                    }
                }
            }
            if (achievement.progress < achievement.data.goalValue)
            {

                progressBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                    -98f + (achievement.progress * (98f / (achievement.data.goalValue))),
                    progressBar.GetComponent<RectTransform>().anchoredPosition.y
                );
                //int absoluteProgress = achievement.progress;
                progressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(
                    -196f + (achievement.progress * (196f / (achievement.data.goalValue))),
                    progressBar.GetComponent<RectTransform>().sizeDelta.y
                );

                string progressMessage = $"{achievement.progress} / {achievement.data.goalValue}";
                if (achievement.data.measurementType == MeasurementType.Level) { progressMessage += " Level(s) Finished"; }
                else if (achievement.data.measurementType == MeasurementType.SecondsRemaining) {
                    progressMessage = $"Completed in {achievement.progress} / {achievement.data.goalValue} Seconds";
                    progressBar.GetComponent<Image>().color = new Color(121f / 255f, 255f / 255f, 122f / 255f); // green
                }
                progressText.GetComponent<TextMeshProUGUI>().text = progressMessage;
            }

            // Button
            if (achievement.progress >= achievement.data.goalValue)
            {
                if (achievement.data.measurementType == MeasurementType.Units || achievement.data.measurementType == MeasurementType.Level)
                {
                    Button achievementButtonOverlay = GameObjectFinder.FindChildRecursive(achievementCard, "Button").GetComponent<Button>();
                    achievementButtonOverlay.interactable = true;
                }
            }
            if (achievement.progress <= achievement.data.goalValue)
            {
                if (achievement.data.measurementType == MeasurementType.SecondsRemaining)
                {
                    Button achievementButtonOverlay = GameObjectFinder.FindChildRecursive(achievementCard, "Button").GetComponent<Button>();
                    achievementButtonOverlay.interactable = true;
                }
            }
        }
    }

    private void DeleteAchievementCards()
    {
        foreach (GameObject achievementCard in achievementCards)
        {
            Destroy(achievementCard);
        }

        achievementCards.Clear();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (lastScene != null) { SceneManager.UnloadSceneAsync(lastScene); }

        Debug.Log("Scene Loaded: " + scene.name);

        DeleteAchievementCards();

        SearchForFeatures();

        if (gameStartMenu != null)
        {
            if (IsAchievementCompleted("Tutorial"))
            {
                Debug.Log("Load Levels Menu");
                isLevelsMenuOpen = false;

                GameObject startGameButton = GameObjectFinder.FindChildRecursive(gameStartMenu, "StartButton");
                GameObject levelsButton = GameObjectFinder.FindChildRecursive(gameStartMenu, "LevelsButton");
                levelsButton.GetComponent<Button>().onClick.AddListener(ToggleLevelMenu);

                startGameButton.SetActive(false);
                levelsButton.SetActive(true);

                GameObject levelMenu = GameObjectFinder.FindChildRecursive(CanvasHandler.Instance.gameObject, "LevelScreen");
                GameObject backButton = GameObjectFinder.FindChildRecursive(levelMenu, "BackButton");
                backButton.GetComponent<Button>().onClick.AddListener(ToggleLevelMenu);
            }
        }
    }
}
