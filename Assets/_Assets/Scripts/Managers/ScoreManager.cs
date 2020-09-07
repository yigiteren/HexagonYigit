using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // Properties //
    public static ScoreManager Instance { get; private set; }
    public int Score { get; private set; }
    public bool ShouldSpawnBomb { get; private set; }
    
    // Editor Variables //
    [SerializeField] private int scoreMultiplier = 5;
    
    // Private Variables
    private int _bombCounter;

    /// <summary>
    /// Adds score based on destroyed tile amount.
    /// </summary>
    /// <param name="destroyedTileAmount">Amount of tiles destroyed</param>
    public void AddScore(int destroyedTileAmount)
    {
        Score += destroyedTileAmount * scoreMultiplier;
        _bombCounter += destroyedTileAmount * scoreMultiplier;

        if (_bombCounter >= 50)
        {
            _bombCounter = 0;
            ShouldSpawnBomb = true;
        }
        
        UIManager.Instance.ChangeScoreText(Score);
    }
    
    /// <summary>
    /// Sets ShouldSpawnBomb to false.
    /// </summary>
    public void DisableBombSpawn()
        => ShouldSpawnBomb = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Score = 0;
        _bombCounter = 0;
    }
}
