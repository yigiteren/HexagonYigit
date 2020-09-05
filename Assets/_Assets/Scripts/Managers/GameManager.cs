using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GridManager.Instance.GenerateHexagonalGrid();
        GridManager.Instance.GenerateCursorGrid();
        HexagonManager.Instance.SpawnInitialHexagons();
        CursorManager.Instance.SpawnCursor();
    }
}
