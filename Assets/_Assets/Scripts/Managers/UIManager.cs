using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    // Editor Variables //
    [Header("UI Variables")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Dialogue Variables")]
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TextMeshProUGUI panelText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    public void ChangeScoreText(int score)
        => scoreText.text = score.ToString();

    /// <summary>
    /// Shows a dialogue box.
    /// </summary>
    /// <param name="mainText">Main text to show</param>
    /// <param name="leftButtonText">Text of the left button</param>
    /// <param name="rightButtonText">Text of the left button</param>
    /// <param name="leftButtonAction">Action of the left button</param>
    /// <param name="rightButtonAction">Action of the right button</param>
    public void ShowDialogueBox(
        string mainText,
        string leftButtonText,
        string rightButtonText,
        Action leftButtonAction,
        Action rightButtonAction)
    {
        panelText.text = mainText;
        leftButton.GetComponentInChildren<TextMeshProUGUI>().text = leftButtonText;
        rightButton.GetComponentInChildren<TextMeshProUGUI>().text = rightButtonText;
        leftButton.onClick.AddListener(leftButtonAction.Invoke);
        rightButton.onClick.AddListener(rightButtonAction.Invoke);
        
        dialogueBox.SetActive(true);
    }

    /// <summary>
    /// Hides the dialogue box and removes the listeners
    /// </summary>
    public void HideDialogueBox()
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();
        
        dialogueBox.SetActive(false);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        dialogueBox.SetActive(false);
    }
}
