using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    // Properties //
    public static HexagonManager Instance { get; private set; }
    public List<HexagonController> HexagonControllers { get; private set; }
    
    // Editor Variables //
    [SerializeField] private GameObject hexagonPrefab;

    /// <summary>
    /// Spawns initial hexagons.
    /// </summary>
    public void SpawnInitialHexagons()
        => StartCoroutine(SpawnInitialHexagonsEnumerator());

    private void Awake()
    {
        // Singleton Protector //
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private IEnumerator SpawnInitialHexagonsEnumerator()
    {
        HexagonControllers = new List<HexagonController>();
        var grid = GridManager.Instance.HexagonGrid;
        var parent = new GameObject("Hexagons").transform;

        for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                var offset = new Vector2(0, 10);
                var position = grid[x, y];
                var rotation = Quaternion.Euler(0, -90, 90);
                var hexagon = Instantiate(hexagonPrefab, position + offset, rotation, parent);
                hexagon.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

                var controller = hexagon.GetComponent<HexagonController>();
                controller.SetIdentifier(new Vector2Int(x, y));
                controller.MoveHexagonTo(position, 1f);
                ColorManager.Instance.ApplySuitableColor(controller);
                
                HexagonControllers.Add(controller);
                
                yield return new WaitForSeconds(0.075f);
            }
    }
}
