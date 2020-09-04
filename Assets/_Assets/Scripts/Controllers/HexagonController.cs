using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    // Public Variables //
    public Vector2Int Identifier;
    public Color Color => _meshRenderer.material.color;
    public List<HexagonController> test;

    // Private Variables //
    private MeshRenderer _meshRenderer;

    /// <summary>
    /// Checks neighbor hexagons and returns a list of excluded colors
    /// to prevent points at start.
    /// </summary>
    /// <returns>Excluded colors</returns>
    public List<Color> GetColorsToExclude()
    {
        var colorsToExclude = new List<Color>();
        var neighborhoods = FindAllNeighborhoodControllers();
        test = neighborhoods;

        for (var i = 0; i < neighborhoods.Count; i++)
        {
            var first = neighborhoods[i];
            var second = i == neighborhoods.Count - 1 ? neighborhoods[0] : neighborhoods[i + 1];
            
            if(ColorManager.Instance.CompareColors(first.Color, second.Color))
                colorsToExclude.Add(first.Color);
        }
        
        Debug.Log(Identifier + " - " + colorsToExclude.Count);
        return colorsToExclude;
    }

    /// <summary>
    /// Returns all neighborhood controllers, starting from the above,
    /// clockwise.
    /// </summary>
    /// <returns></returns>
    public List<HexagonController> FindAllNeighborhoodControllers()
    {
        var neighborhoods = new List<HexagonController>();
        var directions = Enum.GetValues(typeof(Direction)).Cast<Direction>();

        foreach (var direction in directions)
        {
            var controller = FindNeighborhoodController(direction);
            if(controller) neighborhoods.Add(controller);
        }

        return neighborhoods;
    }

    /// <summary>
    /// Finds and returns neighborhood of controller.
    /// </summary>
    public HexagonController FindNeighborhoodController(Direction direction)
        => HexagonManager.Instance.HexagonControllers.FirstOrDefault(controller =>
            controller.Identifier == Identifier + ConvertDirectionToIdentifier(direction));

    /// <summary>
    /// Changes the identifier of the hexagon.
    /// </summary>
    /// <param name="identifier">New identifier</param>
    public void SetIdentifier(Vector2Int identifier)
        => Identifier = identifier;

    /// <summary>
    /// Moves hexagon to target position.
    /// </summary>
    /// <param name="position">Final position.</param>
    /// <param name="duration">Duration of movement.</param>
    public void MoveHexagonTo(Vector2 position, float duration)
        => StartCoroutine(MoveHexagonToEnumerator(position, duration));

    /// <summary>
    /// Applies color to hexagon.
    /// </summary>
    /// <param name="color"></param>
    public void ApplyColor(Color color)
        => _meshRenderer.material.color = color;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Converts given direction to an identifier.
    /// </summary>
    /// <param name="direction">Direction to convert</param>
    /// <returns>Target identifier</returns>
    private Vector2Int ConvertDirectionToIdentifier(Direction direction)
    {
        switch (direction)
        {
            case Direction.Above: 
                return new Vector2Int(0, 1);
            case Direction.AboveLeft:               // TEK //                     // ÇİFT //
                return Identifier.x % 2 != 0 ? new Vector2Int(-1, 0) : new Vector2Int(-1, 1);
            case Direction.AboveRight: 
                return Identifier.x % 2 != 0 ? new Vector2Int(+1, 0) : new Vector2Int(1, 1);
            case Direction.Below: 
                return new Vector2Int(0, -1);
            case Direction.BelowLeft:
                return Identifier.x % 2 != 0 ? new Vector2Int(-1, -1) : new Vector2Int(-1, 0);
            case Direction.BelowRight:
                return Identifier.x % 2 != 0 ? new Vector2Int(1, -1) : new Vector2Int(1, 0);
        }
        
        return new Vector2Int(0, 0);
    }

    private IEnumerator MoveHexagonToEnumerator(Vector2 position, float duration)
    {
        var startPosition = transform.position;
        var endPosition = position;
        var timer = 0f;

        while (timer < duration)
        {
            var interpolator = (timer / duration) * (timer / duration);
            transform.position = Vector2.Lerp(startPosition, endPosition, interpolator);
            
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }
}
