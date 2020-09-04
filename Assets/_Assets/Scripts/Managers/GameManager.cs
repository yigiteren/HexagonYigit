using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        GridManager.Instance.GenerateHexagonalGrid();
        HexagonManager.Instance.SpawnInitialHexagons();
    }
}
