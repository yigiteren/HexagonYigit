using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class HexagonManager : MonoBehaviour
{
    // Properties //
    public static HexagonManager Instance { get; private set; }
    public List<HexagonController> HexagonControllers { get; private set; }
    
    // Events //
    public delegate void OnMoveMadeDelegate();

    public static event OnMoveMadeDelegate OnMoveMade;
    // Editor Variables //
    [SerializeField] private GameObject hexagonPrefab;
    [SerializeField] private GameObject bombPrefab;

    // Private Variables //
    private Transform _hexagonParentTransform;
    private bool _isRotating;

    public HexagonController GetHexagonController(Vector2Int identifier)
        => HexagonControllers.FirstOrDefault(controller => controller.Identifier == identifier);

    /// <summary>
    /// Updates the hexagons.
    /// </summary>
    public IEnumerator UpdateHexagons()
    {
        // Disable the player input while updating.
        InputManager.Instance.DisableInput();
        
        var hexagonsToFallDown = HexagonControllers.Where(controller => 
             !controller.DoesHaveHexagonBelow()).ToList();

        while (hexagonsToFallDown.Count > 0)
        {
            hexagonsToFallDown.ForEach(hexagon =>
            {
                if (hexagon) hexagon.MoveHexagonDown();
            });

            hexagonsToFallDown = HexagonControllers.Where(controller => 
                !controller.DoesHaveHexagonBelow()).ToList();

            yield return null;
        }
        
        yield return new WaitForSeconds(0.25f);

        var hexagonsToDestroy = GetHexagonsToDestroy(HexagonControllers);
        if (hexagonsToDestroy.Count > 0)
        {
            DestroyHexagons(hexagonsToDestroy);
            StartCoroutine(UpdateHexagons());
        }
        else
        {
            OnMoveMade?.Invoke();
            
            var emptyIdentifiers = FindEmptyIdentifiers();
            //ScoreManager.Instance.AddScore(emptyIdentifiers.Count);
            foreach (var emptyIdentifier in emptyIdentifiers)
            {
                if (ScoreManager.Instance.ShouldSpawnBomb)
                {
                    SpawnPrefab(emptyIdentifier, bombPrefab);
                    ScoreManager.Instance.DisableBombSpawn();
                }
                else
                    SpawnPrefab(emptyIdentifier, hexagonPrefab);
                yield return new WaitForSeconds(0.10f);
            }
            
            // Enable player input after update.
            InputManager.Instance.EnableInput();
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
    {
        if(InputManager.Instance.IsInputEnabled)
            StartCoroutine(RotateHexagonsEnumerator(clockwise));
    }

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

    /// <summary>
    /// Spawns given prefab and moves it into the correct position.
    /// </summary>
    /// <param name="identifier"></param>
    private void SpawnPrefab(Vector2Int identifier, GameObject prefab)
    {
        var offset = new Vector2(0, 10);
        var position = GridManager.Instance.GetPositionToMove(identifier);
        var rotation = Quaternion.Euler(0, -90, 90);
        var hexagon = Instantiate(prefab, position + offset, rotation, _hexagonParentTransform);
        hexagon.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

        var controller = hexagon.GetComponent<HexagonController>();
        controller.MoveHexagonTo(position, 0.5f, identifier);
        ColorManager.Instance.ApplySuitableColor(controller);
                
        HexagonControllers.Add(controller);
    }

    /// <summary>
    /// Finds identifiers that are empty.
    /// </summary>
    /// <returns>A list of empty identifiers</returns>
    private List<Vector2Int> FindEmptyIdentifiers()
    {
        var identifiers = GridManager.Instance.GetAllIdentifiers();
        var existingIdentifiers = HexagonControllers.Select(controller => controller.Identifier).ToList();
        var emptyIdentifiers = identifiers.Except(existingIdentifiers).ToList();
        return emptyIdentifiers;
    }
    
    private IEnumerator SpawnInitialHexagonsEnumerator()
    {
        HexagonControllers = new List<HexagonController>();
        var grid = GridManager.Instance.HexagonGrid;
        _hexagonParentTransform = new GameObject("Hexagons").transform;

        for (var x = 0; x < grid.GetLength(0); x++)
            for (var y = 0; y < grid.GetLength(1); y++)
            {
                var offset = new Vector2(0, 15);
                var position = grid[x, y];
                var rotation = Quaternion.Euler(0, -90, 90);
                var hexagon = Instantiate(hexagonPrefab, position + offset, rotation, _hexagonParentTransform);
                hexagon.transform.localScale = new Vector3(0.95f, 0.95f, 0.95f);

                var controller = hexagon.GetComponent<HexagonController>();
                controller.MoveHexagonTo(position, 1f, new Vector2Int(x, y));
                ColorManager.Instance.ApplySuitableColor(controller);
                
                HexagonControllers.Add(controller);
                
                yield return new WaitForSeconds(0.075f);
            }
        
        InputManager.Instance.EnableInput();
    }

    private IEnumerator RotateHexagonsEnumerator(bool clockwise)
    {
        InputManager.Instance.DisableInput();
        var foundMatch = false;
        
        for (var i = 0; i < 3; i++)
        {
            var controllers = CursorManager.Instance.Cursor.GetNearbyHexagons();
            RotateHexagonsOnce(controllers, clockwise);
            yield return new WaitForSeconds(0.25f);

            if (CheckHexagonsToDestroy(controllers))
            {
                foundMatch = true;
                break;
            }
        }
        
        if(!foundMatch)
            InputManager.Instance.EnableInput();
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

        return hexagonsToDestroy.Distinct().ToList();
    }

    /// <summary>
    /// Destroys the given hexagons and removes them from the list.
    /// </summary>
    /// <param name="hexagonsToDestroy">Hexagons to destroy</param>
    private void DestroyHexagons(List<HexagonController> hexagonsToDestroy)
    {
        var controllersToAdd = new List<HexagonController>();
        
        hexagonsToDestroy.ForEach(controller =>
        {
            if (controller is BombController)
            {
                var bomb = (BombController) controller;
                controllersToAdd.AddRange(bomb.GetControllersToDestroy());
            }
        });
        
        hexagonsToDestroy.AddRange(controllersToAdd);
        hexagonsToDestroy.ForEach(controller =>
        {
            HexagonControllers.Remove(controller);
            Destroy(controller.gameObject);
        });
        
        ScoreManager.Instance.AddScore(hexagonsToDestroy.Count);
    }
}
