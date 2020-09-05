using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Properties //
    public static GameManager Instance { get; private set; }

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
