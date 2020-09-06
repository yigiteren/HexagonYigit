using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Properties //
    public static ScoreManager Instance { get; private set; }
    public int Score { get; private set; }
    
    // Editor Variables //
    [SerializeField] private int scoreMultiplier = 5;

    /// <summary>
    /// Adds score based on destroyed tile amount.
    /// </summary>
    /// <param name="destroyedTileAmount">Amount of tiles destroyed</param>
    public void AddScore(int destroyedTileAmount)
    {
        Score += destroyedTileAmount * scoreMultiplier;
        UIManager.Instance.ChangeScoreText(Score);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Score = 0;
    }
}
