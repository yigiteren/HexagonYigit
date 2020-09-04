using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ColorManager : MonoBehaviour
{
    // Properties //
    public static ColorManager Instance { get; private set; }
    
    // Editor Variables //
    [SerializeField] private List<Color> hexagonColors;

    /// <summary>
    /// Applies a suitable color for first time generated hexagon controller.
    /// </summary>
    /// <param name="controller"></param>
    public void ApplySuitableColor(HexagonController controller)
    {
        var colorsToExclude = controller.GetColorsToExclude();
        var suitableColors = new List<Color>();

        // This can be optimized way a lot more however since the time is short
        // I'll just stick with it.
        foreach (var hexagonColor in hexagonColors)
        {
            var areEqual = false;
            foreach (var color in colorsToExclude)
                if (CompareColors(color, hexagonColor))
                    areEqual = true;
            
            if(!areEqual)
                suitableColors.Add(hexagonColor);
        }


        var randomColor = suitableColors[Random.Range(0, suitableColors.Count)];
        controller.ApplyColor(randomColor);
    }

    /// <summary>
    /// Compares two colors by values.
    /// </summary>
    /// <param name="first">First color</param>
    /// <param name="second">Second color</param>
    /// <returns>If they are equal</returns>
    public bool CompareColors(Color first, Color second)
    {
        return Math.Abs(first.r - second.r) < 0.01f &&
               Math.Abs(first.g - second.g) < 0.01f &&
               Math.Abs(first.b - second.b) < 0.01f &&
               Math.Abs(first.a - second.a) < 0.01f;
    }
    
    private void Awake()
    {
        // Singleton Protector //
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
}
