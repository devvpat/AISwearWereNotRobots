using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    // singleton setup
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private enum GameState
    {
        BeforeClass,
        Class,
        Lunch,
        AfterClass,
        Day5AfterClass
    }
    private GameState currentGameState;

    private Dictionary<GameState, string> stateToNiceString = new()
    {
        { GameState.BeforeClass, "Before Class" },
        { GameState.Class, "Class" },
        { GameState.Lunch, "Lunch" },
        { GameState.AfterClass, "After Class" },
        { GameState.Day5AfterClass, "Day 5 After Class" }
    };

    // UI references
    [Header("Settings")]
    [SerializeField] private int startNumberOfKeys = 3;
    [SerializeField] private int newKeysPerDay = 3;

    [Header("Before Class")]
    [SerializeField] private GameObject beforeClassUI;
    [SerializeField] private GameObject[] beforeClassPerson1ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] beforeClassPerson2ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] beforeClassPerson3ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] beforeClassTeacherChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] beforeClassBoardChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject beforeClassWindowHolder;

    [Header("Class")]
    [SerializeField] private GameObject classUI;
    [SerializeField] private GameObject classSlot1;
    [SerializeField] private GameObject classSlot2;
    [SerializeField] private GameObject classBackgroundImage;
    [SerializeField] private Sprite[] classBackgroundSprites = new Sprite[3];
    [SerializeField] private TMP_Text classText;
    [SerializeField] private string[] classTexts = new string[5];

    [Header("Lunch")]
    [SerializeField] private GameObject lunchUI;
    [SerializeField] private GameObject lunchSlot1;
    [SerializeField] private GameObject lunchSlot2;
    [SerializeField] private GameObject lunchBackgroundImage;
    [SerializeField] private Sprite[] lunchBackgroundSprites = new Sprite[3];
    [SerializeField] private TMP_Text lunchText;
    [SerializeField] private string[] lunchTexts = new string[5];

    [Header("After Class")]
    [SerializeField] private GameObject afterClassUI;
    [SerializeField] private GameObject[] afterClassPerson1ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] afterClassPerson2ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] afterClassPerson3ChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] afterClassTeacherChildrenObjects = new GameObject[3];
    [SerializeField] private GameObject[] afterClassBoardChildrenObjects = new GameObject[3];
    [SerializeField] private TMP_Text afterClassText;
    [SerializeField] private GameObject afterClassButton;

    [Header("Day 5 After Class")]
    [SerializeField] private GameObject day5AfterClassUI;

    [Header("Effects")]
    [SerializeField] private Image vignetteImage;
    [SerializeField] private Volume glitchVolume;

    [Header("Other References")]
    [SerializeField] private GameObject wordBank;
    public WordBank wordBankComp;
    [SerializeField] private TMP_Text wbKeysText;
    [SerializeField] private GameObject wordBankButton;
    [SerializeField] private GameObject advanceButton;
    [SerializeField] private GameObject failGameUI;
    [SerializeField] private GameObject clockObject;
    [SerializeField] private TMP_Text clockText;

    // variables
    public int CurrentDay { get; private set; }

    private readonly int maxDays = 5;
    private int numberOfKeys;
    private int socialPoints;
    private int academicPoints;
    private DragAndDropItem.WordType afterClassPointType;
    private int currMinigameWordsUsed;

    void Start()
    {
        SetupStats();
        StartGame();
        SetupCursor();
    }

    private void SetupCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SetupStats()
    {
        CurrentDay = 0;
        socialPoints = 0;
        academicPoints = 0;
        numberOfKeys = startNumberOfKeys;
        SetKeyText();
        currentGameState = GameState.BeforeClass;
    }

    private void StartGame()
    {
        UpdateBeforeClassUI();
        Debug.Log("Game Started");
        PrintStats();

        beforeClassUI.SetActive(true);
        classUI.SetActive(false);
        lunchUI.SetActive(false);
        afterClassUI.SetActive(false);
        day5AfterClassUI.SetActive(false);
        failGameUI.SetActive(false);
        advanceButton.SetActive(false);
        wordBankButton.SetActive(true);

        wordBank.SetActive(true);
        clockObject.SetActive(true);
    }

    public void AdvanceScene()
    {
        switch (currentGameState)
        {
            case GameState.BeforeClass:
                beforeClassUI.SetActive(false);
                UpdateClassUI();
                classUI.SetActive(true);
                currentGameState = GameState.Class;
                PrintStats();
                break;

            case GameState.Class:
                classUI.SetActive(false);
                PlayClassMinigame();
                if (IsGameFailed())
                {
                    FailGame();
                    return;
                }
                UpdateLunchUI();
                lunchUI.SetActive(true);
                currentGameState = GameState.Lunch;
                PrintStats();
                break;

            case GameState.Lunch:
                lunchUI.SetActive(false);
                PlayLunchMinigame();
                if (IsGameFailed())
                {
                    FailGame();
                    return;
                }
                // skip after class on day 5
                if (CurrentDay == maxDays - 1)
                {
                    day5AfterClassUI.SetActive(true);
                    currentGameState = GameState.Day5AfterClass;
                    PrintStats();
                    EndGame();
                }
                // show normal after class for days 1-4
                else
                {
                    UpdateAfterClassUI();
                    afterClassUI.SetActive(true);
                    currentGameState = GameState.AfterClass;
                    PrintStats();
                }
                break;

            case GameState.AfterClass:
                afterClassUI.SetActive(false);
                UpdateBeforeClassUI();
                beforeClassUI.SetActive(true);
                currentGameState = GameState.BeforeClass;
                AdvanceDay();
                PrintStats();
                break;
        }
        // Update clock text
        clockText.text = $"Day {CurrentDay + 1}\n{stateToNiceString[currentGameState]}";
    }

    private void AdvanceDay()
    {
        CurrentDay++;
        AddKey(newKeysPerDay);
    }

    private void EndGame()
    {
        wordBank.SetActive(false);
        wordBankButton.SetActive(false);
        advanceButton.SetActive(false);
        clockObject.SetActive(false);
        Debug.Log("Game Over");
    }

    private bool IsGameFailed()
    {
        return socialPoints > 2 || academicPoints > 2;
    }

    private void FailGame()
    {
        Debug.Log("Game Failed");
        wordBank.SetActive(false);
        wordBankButton.SetActive(false);
        advanceButton.SetActive(false);
        clockObject.SetActive(false);
        failGameUI.SetActive(true);
    }

    private void PlayClassMinigame()
    {
        Debug.Log("Playing Class Minigame");
        int score = 0;
        score += EvaluateSlot(classSlot1, DragAndDropItem.WordType.Academic);
        score += EvaluateSlot(classSlot2, DragAndDropItem.WordType.Academic);
        academicPoints += score == -2 ? -1 : 1; // -1 if both slots correct, else +1
        // clamp academicPoints to [0, 3] (inclusive)
        academicPoints = Mathf.Clamp(academicPoints, 0, 3);
        Debug.Log($"Class Minigame Score: {score}, Academic Points: {academicPoints}");
    }

    private void PlayLunchMinigame()
    {
        Debug.Log("Playing Lunch Minigame");
        int score = 0;
        score += EvaluateSlot(lunchSlot1, DragAndDropItem.WordType.Social);
        score += EvaluateSlot(lunchSlot2, DragAndDropItem.WordType.Social);
        socialPoints += score == -2 ? -1 : 1; // -1 if both slots correct, else +1
        // clamp socialPoints to [0, 3] (inclusive)
        socialPoints = Mathf.Clamp(socialPoints, 0, 3);
        Debug.Log($"Lunch Minigame Score: {score}, Social Points: {socialPoints}");
    }

    private int EvaluateSlot(GameObject slot, DragAndDropItem.WordType wordType)
    {
        int val;
        if (slot.transform.childCount != 1) val = 1; // Incorrect
        else
        {
            GameObject child = slot.transform.GetChild(0).gameObject;
            child.TryGetComponent<DragAndDropItem>(out DragAndDropItem item);
            if (item == null) val = 1; // Incorrect
            else if (item.wordType == wordType) val = -1; // Correct
            else val = 1; // Incorrect
        }
        // remove all children of slot
        foreach (Transform child in slot.transform)
        {
            Destroy(child.gameObject);
        }
        return val;
    }

    private void UpdateBeforeClassUI()
    {
        Debug.Log("Updating Before Class UI");
        DeactivateAllChildren(beforeClassWindowHolder);
        advanceButton.SetActive(false);
        UpdateImageGameObject(beforeClassPerson1ChildrenObjects, socialPoints);
        UpdateImageGameObject(beforeClassPerson2ChildrenObjects, socialPoints);
        UpdateImageGameObject(beforeClassPerson3ChildrenObjects, socialPoints);
        UpdateImageGameObject(beforeClassTeacherChildrenObjects, academicPoints);
        UpdateImageGameObject(beforeClassBoardChildrenObjects, socialPoints);

        UpdateEffects();
    }

    private void DeactivateAllChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void UpdateClassUI()
    {
        Debug.Log("Updating Class UI");
        UpdateImage(classBackgroundImage, classBackgroundSprites, academicPoints);
        // enable advance button if word bank is empty otherwise disable it
        advanceButton.SetActive(wordBankComp.IsEmpty());
        currMinigameWordsUsed = 0; // reset words used for class minigame
        classText.text = classTexts[CurrentDay];

        UpdateEffects();
    }

    private void UpdateLunchUI()
    {
        Debug.Log("Updating Lunch UI");
        UpdateImage(lunchBackgroundImage, lunchBackgroundSprites, socialPoints);
        // enable advance button if word bank is empty otherwise disable it
        advanceButton.SetActive(wordBankComp.IsEmpty());
        currMinigameWordsUsed = 0; // reset words used for lunch minigame
        lunchText.text = lunchTexts[CurrentDay];

        UpdateEffects();
    }

    private void UpdateAfterClassUI()
    {
        Debug.Log("Updating After Class UI");
        advanceButton.SetActive(false);
        UpdateImageGameObject(afterClassPerson1ChildrenObjects, socialPoints);
        UpdateImageGameObject(afterClassPerson2ChildrenObjects, socialPoints);
        UpdateImageGameObject(afterClassPerson3ChildrenObjects, socialPoints);
        UpdateImageGameObject(afterClassTeacherChildrenObjects, academicPoints);
        UpdateImageGameObject(afterClassBoardChildrenObjects, socialPoints);

        int res = UnityEngine.Random.Range(0, 2);
        if (res == 0) afterClassPointType = DragAndDropItem.WordType.Social;
        else afterClassPointType = DragAndDropItem.WordType.Academic;
        afterClassText.text = $"Would you like to participate in {(afterClassPointType == DragAndDropItem.WordType.Social ? "a" : "an")} " +
                              $"{afterClassPointType.ToString().ToLower()} after class activity? The words in your word bank will alter afterward.";

        UpdateEffects();
    }

    public void OnAfterClassButtonClick()
    {
        int num = UnityEngine.Random.Range(1, 3); // 1 or 2
        for (int i = 0; i < num; i++)
        {
            if (afterClassPointType == DragAndDropItem.WordType.Social) wordBankComp.TryRemoveWord(DragAndDropItem.WordType.Academic);
            else wordBankComp.TryRemoveWord(DragAndDropItem.WordType.Social);

            wordBankComp.TryAddWord(afterClassPointType);
        }
        AdvanceScene();
        Debug.Log($"After Class Button Clicked: Added {num} {afterClassPointType} word(s) and removed {num} {(afterClassPointType == DragAndDropItem.WordType.Social ? "academic" : "social")} word(s).");
    }

    private void UpdateImage(GameObject obj, Sprite[] sprites, int index)
    {
        if (index < 0 || index >= sprites.Length)
        {
            Debug.LogError($"Index {index} out of bounds for sprites array on {obj.name}");
            return;
        }
        if (obj.TryGetComponent<Image>(out Image img))
        {
            if (sprites[index] == null)
            {
                Debug.LogError($"Sprite at index {index} is null during UpdateImage on {obj.name}");
                return;
            }
            img.sprite = sprites[index];
        }
        else
        {
            Debug.LogError($"Image component not found on {obj.name}");
        }
    }

    private void UpdateImageGameObject(GameObject[] children, int index)
    {
        if (index < 0 || index >= children.Length)
        {
            Debug.LogError($"Index {index} out of bounds for children array");
            return;
        }
        foreach (GameObject child in children)
        {
            if (child != null)
                child.SetActive(false);
        }
        if (children[index] != null)
        {
            children[index].SetActive(true);
            Debug.Log($"Updated object with child at index {index}");
        }
        else
        {
            Debug.LogError($"Child at index {index} is null during UpdateImageGameObject");
        }
    }

    private void PrintStats()
    {
        Debug.Log($"Day {CurrentDay + 1} - {currentGameState}, Social Points: {socialPoints}, Academic Points: {academicPoints}, Keys: {numberOfKeys}");
    }

    public bool TryUseKey()
    {
        if (numberOfKeys > 0)
        {
            numberOfKeys--;
            if (wbKeysText) wbKeysText.text = $"x{numberOfKeys}";
            Debug.Log($"Used a key. Remaining keys: {numberOfKeys}");
            if (numberOfKeys == 0 && currentGameState == GameState.BeforeClass)
            {
                advanceButton.SetActive(true);
            }
            return true;
        }
        Debug.Log("No keys left to use.");
        return false;
    }

    public void OnUseWordFromWordBankDuringMinigame()
    {
        if (currentGameState == GameState.Class || currentGameState == GameState.Lunch)
        {
            Debug.Log("Word used from word bank during minigame.");
            currMinigameWordsUsed++;
            Debug.Log($"Current Minigame Words Used: {currMinigameWordsUsed}");
            // if num used words == 1 and word bank is empty, enable advance button
            if (currMinigameWordsUsed == 1 && wordBankComp.IsEmpty())
            {
                advanceButton.SetActive(true);
            }
            // if num used words == 2, enable advance button
            else if (currMinigameWordsUsed == 2)
            {
                advanceButton.SetActive(true);
            }
        }
    }
    public void AddKey(int amount)
    {
        numberOfKeys += amount;
        SetKeyText();
    }

    private void SetKeyText()
    {
        if (wbKeysText) wbKeysText.text = $"x{numberOfKeys}";
    }
    private void UpdateEffects()
    {
        // Vignette (fades in at 1+)
        bool showVignette = socialPoints >= 1 || academicPoints >= 1;
        bool hideVignette = socialPoints < 1 && academicPoints < 1;

        // Glitch (activates at 2+)
        bool showGlitch = socialPoints >= 2 || academicPoints >= 2;
        bool hideGlitch = socialPoints < 2 && academicPoints < 2;

        // Update vignette alpha
        Color color = vignetteImage.color;

        if (showVignette && color.a == 0f)
        {
            color.a = 1f;
            vignetteImage.color = color;
        }
        else if (hideVignette && color.a > 0f)
        {
            color.a = 0f;
            vignetteImage.color = color;
        }

        // Enable/disable glitch volume
        if (glitchVolume != null)
        {
            if (showGlitch && !glitchVolume.enabled)
            {
                glitchVolume.enabled = true;
            }
            else if (hideGlitch && glitchVolume.enabled)
            {
                glitchVolume.enabled = false;
            }
        }
    }
}

