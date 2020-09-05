using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Properties //
    public static InputManager Instance { get; private set; }

    // Editor Variables //
    [SerializeField] private Camera gameCamera;
        
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
            ChangeCursorPosition();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HexagonManager.Instance.RotateHexagons(true);
        }
    }

    private void ChangeCursorPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        mousePosition = gameCamera.ScreenToWorldPoint(mousePosition);

        CursorManager.Instance.MoveCursorToNearestSnapPoint(mousePosition);
    }
}