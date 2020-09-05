using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    // Properties //
    public static GridManager Instance { get; private set; }
    public Vector2[,] HexagonGrid { get; private set; }
    public List<Vector2> CursorGrid { get; private set; }
    
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

    /// <summary>
    /// Generates the grid for cursor to snap.
    /// </summary>
    public void GenerateCursorGrid()
    {
        CursorGrid = new List<Vector2>();
        
        var firstSubGrid = GenerateCursorSubGrid(new Vector2(0.25f, 0.433f), 1f);
        var secondSubGrid = GenerateCursorSubGrid(new Vector2(0.5f, 0f), 0.5f);
        
        CursorGrid.AddRange(firstSubGrid);
        CursorGrid.AddRange(secondSubGrid);
    }

    /// <summary>
    /// Returns the nearest position on the cursor grid to snap the cursor.
    /// </summary>
    /// <param name="location"></param>
    /// <returns></returns>
    public Vector2 FindNearestCursorSnapPoint(Vector2 location)
    {
        var minDistance = Vector2.Distance(location, CursorGrid[0]);
        var selectedPoint = CursorGrid[0];
        
        CursorGrid.ForEach(snapPoint =>
        {
            var distance = Vector2.Distance(location, snapPoint);
            if(distance < minDistance)
            {
                minDistance = distance;
                selectedPoint = snapPoint;
            }
        });

        return selectedPoint;
    }

    /// <summary>
    /// Generates a grid at given start point.
    /// </summary>
    /// <param name="startPosition">Start position of the grid</param>
    /// <param name="increment">Amount to increase X value</param>
    private List<Vector2> GenerateCursorSubGrid(Vector2 startPosition, float increment)
    {
        var subGridPositions = new List<Vector2>();
        var position = startPosition;
        var xIncrement = increment;
        for (var i = 0; i < gridSize.x - 1; i++)
        {
            for (var j = 0; j < gridSize.y - 1; j++)
            {
                //var rotation = Quaternion.identity;
                //Instantiate(testPrefab, position, rotation);
                subGridPositions.Add(position);
                position.y += 0.866f;
            }

            position.y = startPosition.y;
            position.x += xIncrement;
            xIncrement = Math.Abs(xIncrement - 1f) < 0.01f ? 0.5f : 1f;
        }

        return subGridPositions;
    }

}
