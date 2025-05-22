using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

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

    // UI references
    [Header("Settings")]
    [SerializeField] private int startNumberOfKeys = 3;

    [Header("Image References: 0 = Normal ... 2 = Angry")]
    [SerializeField] private Image[] person1Images = new Image[3];
    [SerializeField] private Image[] person2Images = new Image[3];
    [SerializeField] private Image[] person3Images = new Image[3];
    [SerializeField] private Image[] teacherImages = new Image[3];

    [Header("Before Class")]
    [SerializeField] private GameObject beforeClassUI;
    [SerializeField] private GameObject beforeClassPerson1;
    [SerializeField] private GameObject beforeClassPerson2;
    [SerializeField] private GameObject beforeClassPerson3;
    [SerializeField] private GameObject beforeClassTeacher;
    
    [Header("Class")]
    [SerializeField] private GameObject classUI;
    [SerializeField] private GameObject classTeacher;
    [SerializeField] private GameObject classSlot1;
    [SerializeField] private GameObject classSlot2;

    [Header("Lunch")]
    [SerializeField] private GameObject lunchUI;
    [SerializeField] private GameObject lunchPerson1;
    [SerializeField] private GameObject lunchPerson2;
    [SerializeField] private GameObject lunchPerson3;
    [SerializeField] private GameObject lunchSlot1;
    [SerializeField] private GameObject lunchSlot2;

    [Header("After Class")]
    [SerializeField] private GameObject afterClassUI;
    [SerializeField] private GameObject afterClassPerson1;
    [SerializeField] private GameObject afterClassPerson2;
    [SerializeField] private GameObject afterClassPerson3;
    [SerializeField] private GameObject afterClassTeacher;

    [Header("Day 5 After Class")]
    [SerializeField] private GameObject day5AfterClassUI;

    [Header("Other References")]
    [SerializeField] private GameObject wordBank;
    [SerializeField] private TMP_Text wbKeysText;
    [SerializeField] private GameObject wordBankButton;
    [SerializeField] private GameObject advanceButton;

    // variables
    public int CurrentDay { get; private set; }

    private readonly int maxDays = 5;
    private int numberOfKeys;
    private int socialPoints;
    private int academicPoints;

    void Start()
    {
        SetupCursor();
        SetupStats();
        StartGame();
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
        if (wbKeysText) wbKeysText.text = $"Keys Left: {numberOfKeys}";
        currentGameState = GameState.BeforeClass;
    }

    private void StartGame()
    {
        Debug.Log("Game Started");
        PrintStats();

        beforeClassUI.SetActive(true);
        classUI.SetActive(false);
        lunchUI.SetActive(false);
        afterClassUI.SetActive(false);
        day5AfterClassUI.SetActive(false);

        wordBank.SetActive(true);
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
                UpdateLunchUI();
                lunchUI.SetActive(true);
                currentGameState = GameState.Lunch;
                PrintStats();
                break;

            case GameState.Lunch:
                if (CurrentDay == maxDays - 1)
                {
                    lunchUI.SetActive(false);
                    PlayLunchMinigame();
                    day5AfterClassUI.SetActive(true);
                    currentGameState = GameState.Day5AfterClass;
                    PrintStats();
                    EndGame();
                }
                else
                {
                    lunchUI.SetActive(false);
                    PlayLunchMinigame();
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
    }

    private void AdvanceDay()
    {
        CurrentDay++;
    }

    private void EndGame()
    {
        wordBank.SetActive(false);
        wordBankButton.SetActive(false);
        advanceButton.SetActive(false);
        Debug.Log("Game Over");
    }

    private void FailGame()
    {
        Debug.Log("Game Failed");
    }

    private void PlayClassMinigame()
    {
        Debug.Log("Playing Class Minigame");
        academicPoints += Evaluateslot(classSlot1, DragAndDropItem.WordType.Academic);
        academicPoints += Evaluateslot(classSlot2, DragAndDropItem.WordType.Academic);
        // clamp academicPoints to [0, 3] (inclusive)
        academicPoints = Mathf.Clamp(academicPoints, 0, 3);
    }

    private void PlayLunchMinigame()
    {
        Debug.Log("Playing Lunch Minigame");
        socialPoints += Evaluateslot(lunchSlot1, DragAndDropItem.WordType.Social);
        socialPoints += Evaluateslot(lunchSlot2, DragAndDropItem.WordType.Social);
        // clamp socialPoints to [0, 3] (inclusive)
        socialPoints = Mathf.Clamp(socialPoints, 0, 3);
    }

    private int Evaluateslot(GameObject slot, DragAndDropItem.WordType wordType)
    {
        if (slot.transform.childCount != 1) return 0;

        GameObject child = slot.transform.GetChild(0).gameObject;
        DragAndDropItem item = child.GetComponent<DragAndDropItem>();
        
        if (item.wordType == wordType) return -1; // Correct
        else return 1;  // Incorrect
    }

    private void UpdateBeforeClassUI()
    {
        Debug.Log("Updating Before Class UI");
    }

    private void UpdateClassUI()
    {
        Debug.Log("Updating Class UI");
    }

    private void UpdateLunchUI()
    {
        Debug.Log("Updating Lunch UI");
    }

    private void UpdateAfterClassUI()
    {
        Debug.Log("Updating After Class UI");
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
            if (wbKeysText) wbKeysText.text = $"Keys Left: {numberOfKeys}";
            Debug.Log($"Used a key. Remaining keys: {numberOfKeys}");
            return true;
        }
        Debug.Log("No keys left to use.");
        return false;
    }

    public void AddKey(int amount)
    {
        numberOfKeys += amount;
        wbKeysText.text = $"Number of Keys: {numberOfKeys}";
    }
}
