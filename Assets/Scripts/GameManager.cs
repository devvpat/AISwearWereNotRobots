using UnityEngine;
using TMPro;
using System;

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

    // UI references
    [Header("Settings")]
    [SerializeField] private int startNumberOfKeys = 3;
    [SerializeField] private int startNumberOfDays = 5;

    [Space(25)]

    [Header("Before Class")]
    [SerializeField] private GameObject beforeClassUI;
    
    [Header("Class")]
    [SerializeField] private GameObject classUI;

    [Header("Lunch")]
    [SerializeField] private GameObject lunchUI;

    [Header("After Class")]
    [SerializeField] private GameObject afterClassUI;

    [Header("Day 5 After Class")]
    [SerializeField] private GameObject day5AfterClassUI;

    [Header("Other References")]
    [SerializeField] private GameObject wordBank;
    [SerializeField] private TMP_Text wbKeysText;

    // variables
    private int currentDay;
    private int numberOfKeys;

    void Start()
    {
        SetupCursor();
        SetupStats();
    }

    private void SetupCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SetupStats()
    {
        currentDay = 1;
        numberOfKeys = startNumberOfKeys;
    }

    public bool TryUseKey()
    {
        if (numberOfKeys > 0)
        {
            numberOfKeys--;
            if (wbKeysText) wbKeysText.text = $"Number of Keys: {numberOfKeys}";
            return true;
        }
        return false;
    }

    public void AddKey(int amount)
    {
        numberOfKeys += amount;
        wbKeysText.text = $"Number of Keys: {numberOfKeys}";
    }
}
