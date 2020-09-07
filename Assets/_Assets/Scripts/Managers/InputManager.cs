using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Properties //
    public static InputManager Instance { get; private set; }
    public bool IsInputEnabled => _enableInput;

    // Editor Variables //
    [SerializeField] private Camera gameCamera;
    
    // Private Variables //
    private bool _enableInput;

    /// <summary>
    /// Enables the input and shows the cursor.
    /// </summary>
    public void EnableInput()
    {
        CursorManager.Instance.EnableCursor();
        _enableInput = true;
    }

    /// <summary>
    /// Disables the input and hides the cursor.
    /// </summary>
    public void DisableInput()
    {
        CursorManager.Instance.DisableCursor();
        _enableInput = false;
    }
        
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (!_enableInput) return;

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