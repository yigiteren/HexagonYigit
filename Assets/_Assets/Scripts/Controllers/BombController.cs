using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BombController : HexagonController
{
    // Public Variables //
    public override Color Color => spriteRenderer.color;
    
    // Editor Variables //
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Private Variables //
    private int _movesLeft;

    public List<HexagonController> GetControllersToDestroy()
    {
        var hexagonsToDestroy = HexagonManager.Instance.HexagonControllers
            .Where(controller => ColorManager.CompareColors(controller.Color, Color)).ToList();
        
        hexagonsToDestroy.Add(this);
        return hexagonsToDestroy;
    }

    /// <summary>
    /// Applies color to sprite renderer.
    /// </summary>
    /// <param name="color">Color to apply</param>
    public override void ApplyColor(Color color)
        => spriteRenderer.color = color;

    private void Start()
    {
        _movesLeft = 7;
        counterText.text = _movesLeft.ToString();

        HexagonManager.OnMoveMade += ReduceCounter;
    }
    
    private void ReduceCounter()
    {
        _movesLeft--;
        counterText.text = _movesLeft.ToString();

        if (_movesLeft <= 0)
            GameManager.Instance.DisplayGameOver();
        
    }

    private void OnDestroy()
    {
        if (HexagonManager.Instance == null) return;
        HexagonManager.OnMoveMade -= ReduceCounter;
    }
}
