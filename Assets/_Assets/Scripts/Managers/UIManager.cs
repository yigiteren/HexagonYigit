using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    // Editor Variables //
    [SerializeField] private TextMeshProUGUI scoreText;

    public void ChangeScoreText(int score)
        => scoreText.text = score.ToString();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }
}
