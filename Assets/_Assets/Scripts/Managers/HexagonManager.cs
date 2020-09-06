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
    /// Updates the hexagons.
    /// </summary>
    public IEnumerator UpdateHexagons()
    {
        var hexagonsToUpdate = HexagonControllers.Where(controller => 
            !controller.DoesHaveHexagonBelow()).ToList();

        while (hexagonsToUpdate.Count > 0)
        {
            hexagonsToUpdate.ForEach(controller => controller.MoveHexagonDown());
            yield return new WaitForSeconds(0.3f);
            
            CheckHexagonsToDestroy(hexagonsToUpdate);

            hexagonsToUpdate = HexagonControllers.Where(controller => 
                !controller.DoesHaveHexagonBelow()).ToList();
        }
    }

    /// <summary>
    /// Spawns initial hexagons.
    /// </summary>
    public void SpawnInitialHexagons()
        => StartCoroutine(SpawnInitialHexagonsEnumerator());

    /// <summary>
    /// Rotates the hexagons 3 times, stops if something fits
    /// into correct spot.
    /// </summary>
    /// <param name="controllers">Controllers to rotate</param>
    /// <param name="clockwise">Should rotate clockwise?</param>
    public void RotateHexagons(bool clockwise)
        => StartCoroutine(RotateHexagonsEnumerator(clockwise));

    /// <summary>
    /// Rotates the given hexagons, also handles orientation of the hexagons
    /// like this two:
    ///
    /// x                    x
    ///     x  -  and -  x
    /// x                    x
    /// 
    /// </summary>
    public void RotateHexagonsOnce(List<HexagonController> controllers, bool clockwise)
    {
        // We can only perform this action if there are 3 controllers.
        if (controllers.Count != 3) return;
        var invertRotation = clockwise ? 
            controllers[1].transform.position.x < controllers[0].transform.position.x : 
            controllers[1].transform.position.x > controllers[0].transform.position.x;

        if (invertRotation)
        {
            var firstIdentifierInverted = controllers[2].Identifier;
            for (var i = controllers.Count - 1; i >= 0; i--)
            {
                var controller = controllers[i];
                var targetPos = i == 0 ? 
                    controllers[2].transform.position : 
                    controllers[i - 1].transform.position;

                var identifier = i == 0 ? 
                    firstIdentifierInverted : controllers[i - 1].Identifier;
                
                controller.MoveHexagonTo(targetPos, 0.25f, identifier);
            }
            
            return;
        }

        var firstIdentifier = controllers[0].Identifier;
        for (var i = 0; i < controllers.Count; i++)
        {
            var controller = controllers[i];
            var targetPos = i == controllers.Count - 1 ? 
                controllers[0].transform.position : 
                controllers[i + 1].transform.position;

            var identifier = i == controllers.Count - 1 ? 
                firstIdentifier : controllers[i + 1].Identifier;
            
            controller.MoveHexagonTo(targetPos, 0.25f, identifier);
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
                controller.MoveHexagonTo(position, 1f, new Vector2Int(x, y));
                ColorManager.Instance.ApplySuitableColor(controller);
                
                HexagonControllers.Add(controller);
                
                yield return new WaitForSeconds(0.075f);
            }
    }

    private IEnumerator RotateHexagonsEnumerator(bool clockwise)
    {
        for (var i = 0; i < 3; i++)
        {
            var controllers = CursorManager.Instance.Cursor.GetNearbyHexagons();
            RotateHexagonsOnce(controllers, clockwise);
            yield return new WaitForSeconds(0.25f);

            if (CheckHexagonsToDestroy(controllers))
                break;
        }
    }

    /// <summary>
    /// Checks hexagons to destroy. Returns true if any destroyed.
    /// </summary>
    /// <returns></returns>
    private bool CheckHexagonsToDestroy(List<HexagonController> controllers)
    {
        var hexagonsToDestroy = GetHexagonsToDestroy(controllers);
        if (hexagonsToDestroy.Count > 0)
        {
            DestroyHexagons(hexagonsToDestroy);
            StartCoroutine(UpdateHexagons());
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks each rotated controller to get hexagons to destroy.
    /// </summary>
    /// <returns>Hexagon controllers required to destroy</returns>
    private List<HexagonController> GetHexagonsToDestroy(List<HexagonController> controllersToCheck)
    {
        var hexagonsToDestroy = new List<HexagonController>();
        controllersToCheck.ForEach(controller =>
        {
            var matches = controller.GetAdjacentNeighborsWithSameColor();
                
            if(matches.Count >= 3)
                hexagonsToDestroy.AddRange(matches);
        });

        return hexagonsToDestroy;
    }

    /// <summary>
    /// Destroys the given hexagons and removes them from the list.
    /// </summary>
    /// <param name="hexagonsToDestroy">Hexagons to destroy</param>
    private void DestroyHexagons(List<HexagonController> hexagonsToDestroy)
    {
        hexagonsToDestroy.ForEach(hexagon =>
        {
            HexagonControllers.Remove(hexagon);
            Destroy(hexagon.gameObject);
        });
    }
}
