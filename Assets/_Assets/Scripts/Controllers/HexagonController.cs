using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    // Properties //
    public Vector2Int Identifier { get; private set; }
    public virtual Color Color => _meshRenderer.material.color;

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

        for (var i = 0; i < neighborhoods.Count; i++)
        {
            var first = neighborhoods[i];
            var second = i == neighborhoods.Count - 1 ? neighborhoods[0] : neighborhoods[i + 1];
            
            if(ColorManager.CompareColors(first.Color, second.Color))
                colorsToExclude.Add(first.Color);
        }
        
        return colorsToExclude;
    }

    /// <summary>
    /// Checks if adjacent neighbors have the same color as this one.
    /// </summary>
    /// <returns>Adjacent neighbors with same color. (Includes himself!)</returns>
    public List<HexagonController> GetAdjacentNeighborsWithSameColor()
    {
        var matches = new List<HexagonController>();
        var values = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToList();

        for (var i = 0; i < values.Count; i++)
        {
            var firstDir = values[i];
            var secondDir = i == values.Count - 1 ? values[0] : values[i + 1];

            var firstController =
                HexagonManager.Instance.GetHexagonController(Identifier + ConvertDirectionToIdentifier(firstDir));
            var secondController =
                HexagonManager.Instance.GetHexagonController(Identifier + ConvertDirectionToIdentifier(secondDir));

            if (firstController && secondController &&
                ColorManager.CompareColors(firstController.Color, secondController.Color, Color))
            {
                matches.Add(firstController);
                matches.Add(secondController);
            }
        }

        matches.Add(this);
        return matches;
    }

    /// <summary>
    /// Checks whether if there is a hexagon below or not.
    /// </summary>
    /// <returns></returns>
    public bool DoesHaveHexagonBelow()
    {
        if (Identifier.y == 0) return true;
        return FindNeighborhoodController(Direction.Below);
    }

    /// <summary>
    /// Moves hexagon down 1 unit.
    /// </summary>
    public void MoveHexagonDown()
    {
        var identifierBelow = Identifier + ConvertDirectionToIdentifier(Direction.Below);
        if (identifierBelow.y < 0) return;
        
        var positionToMove = GridManager.Instance.GetPositionToMove(identifierBelow);
        MoveHexagonTo(positionToMove, 0.25f, identifierBelow);
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
    /// Moves hexagon to target position. Identifier must change after moving the hexagon.
    /// </summary>
    /// <param name="position">Final position.</param>
    /// <param name="duration">Duration of movement.</param>
    /// <param name="newIdentifier">New identifier hexagon.</param>
    public void MoveHexagonTo(Vector2 position, float duration, Vector2Int newIdentifier)
    {
        Identifier = newIdentifier;
        StartCoroutine(MoveHexagonToEnumerator(position, duration));
    }

    /// <summary>
    /// Applies color to hexagon.
    /// </summary>
    /// <param name="color"></param>
    public virtual void ApplyColor(Color color)
        => _meshRenderer.material.color = color;

    protected virtual void Awake()
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
            case Direction.AboveLeft:
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
