using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Properties //
    public static GameManager Instance { get; private set; }
    public bool IsGameOver { get; private set; }

    public void ShowResetDialogue()
    {
        UIManager.Instance.ShowDialogueBox(
            "Are you sure you want to reset the level?",
            "Cancel",
            "Yes",
            UIManager.Instance.HideDialogueBox,
            ResetGame
        );
    }

    public void DisplayGameOver()
    {
        IsGameOver = true;
        
        UIManager.Instance.ShowDialogueBox(
            "Game over! Your final score is " + ScoreManager.Instance.Score,
            "Quit",
            "Restart",
            Application.Quit,
            ResetGame
        );
    }

    private void ResetGame()
    {
        GridManager.Instance.ResetManager();
        HexagonManager.Instance.ResetManager();
        ScoreManager.Instance.ResetManager();
        CursorManager.Instance.ResetManager();
        InputManager.Instance.ResetManager();
        ResetManager();
        
        SceneManager.LoadScene(0);
    }

    private void ResetManager()
    {
        IsGameOver = false;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    private void Start()
    {
        GridManager.Instance.GenerateHexagonalGrid();
        GridManager.Instance.GenerateCursorGrid();
        HexagonManager.Instance.SpawnInitialHexagons();
        CursorManager.Instance.SpawnCursor();
    }
}
