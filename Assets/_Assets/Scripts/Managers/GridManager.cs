using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Properties //
    public static GridManager Instance { get; private set; }
    public Vector2[,] HexagonGrid { get; private set; }
    
    // Private Variables //
    [Header("Grid Settings")]
    [SerializeField] private Vector2 gridSize;
    
    private void Awake()
    {
        // Singleton Protector //
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    
    /// <summary>
    /// Generates the hexagonal grid.
    /// </summary>
    public void GenerateHexagonalGrid()
    {
        HexagonGrid = new Vector2[(int)gridSize.x, (int)gridSize.y];
        
        for(var x = 0; x < gridSize.x; x++)
            for (var y = 0; y < gridSize.y; y++)
            {
                var position = new Vector2(x * 0.75f, y * 0.866f);

                if (x % 2 != 0)
                    position.y -= 0.433f;

                HexagonGrid[x, y] = position;
            }
    }

}
