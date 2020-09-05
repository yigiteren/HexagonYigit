using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonManager : MonoBehaviour
{
    // Properties //
    public static HexagonManager Instance { get; private set; }
    public List<HexagonController> HexagonControllers { get; private set; }
    
    // Editor Variables //
    [SerializeField] private GameObject hexagonPrefab;

    public HexagonController GetHexagonController(Vector2Int identifier)
        => HexagonControllers.FirstOrDefault(controller => controller.Identifier == identifier);

    /// <summary>
    /// Spawns initial hexagons.
    /// </summary>
    public void SpawnInitialHexagons()
        => StartCoroutine(SpawnInitialHexagonsEnumerator());

    /// <summary>
    /// Rotates the given hexagons to the right.
    /// </summary>
    public void RotateHexagons(List<HexagonController> controllers, bool clockwise)
    {
        // We can only perform this action if there are 3 controllers.
        if (controllers.Count != 3) return;
        var invertRotation = clockwise ? 
            controllers[1].transform.position.x < controllers[0].transform.position.x : 
            controllers[1].transform.position.x > controllers[0].transform.position.x;

        if (invertRotation)
        {
            for (var i = controllers.Count - 1; i >= 0; i--)
            {
                var controller = controllers[i];
                var targetPos = i == 0 ? 
                    controllers[2].transform.position : 
                    controllers[i - 1].transform.position;
            
                controller.MoveHexagonTo(targetPos, 0.25f);
            }
            
            return;
        }

        for (var i = 0; i < controllers.Count; i++)
        {
            var controller = controllers[i];
            var targetPos = i == controllers.Count - 1 ? 
                controllers[0].transform.position : 
                controllers[i + 1].transform.position;
            
            controller.MoveHexagonTo(targetPos, 0.25f);
        }
    }

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
                var offset = new Vector2(0, 15);
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
