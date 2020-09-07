using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Properties //
    public static InputManager Instance { get; private set; }
    public bool IsInputEnabled => _enableInput;

    // Editor Variables //
    [SerializeField] private Camera gameCamera;
    [SerializeField] private float swipeDetectionThreshold = 1.5f;

    // Private Variables //
    private bool _enableInput;
    private bool _cancelNextMove;

    private Vector3 _initialTouchPosition;

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
        if (!_enableInput || GameManager.Instance && GameManager.Instance.IsGameOver) return;

        if (Input.GetMouseButtonUp(0))
        {
            if(!_cancelNextMove)
                ChangeCursorPosition();

            _cancelNextMove = false;
        }

        if (Input.GetMouseButtonDown(0))
            _initialTouchPosition = MousePositionToWorldPosition();

        if (Input.GetMouseButton(0))
            CheckPlayerSwipe();
    }

    private void CheckPlayerSwipe()
    {
        var mouseWorldPosition = MousePositionToWorldPosition();
        var mouseDelta = mouseWorldPosition - _initialTouchPosition;
        var cursorPosition = CursorManager.Instance.Cursor.transform.position;

        if (mouseDelta.y > swipeDetectionThreshold)
        {
            if (mouseWorldPosition.x < cursorPosition.x)
            {
                HexagonManager.Instance.RotateHexagons(true);
            }
            else
            {
                HexagonManager.Instance.RotateHexagons(false);
            }
        }
        else if (mouseDelta.y < -swipeDetectionThreshold)
        {
            if (mouseWorldPosition.x < cursorPosition.x)
            {
                HexagonManager.Instance.RotateHexagons(false);
            }
            else
            {
                HexagonManager.Instance.RotateHexagons(true);
            }
        }
            
        if (mouseDelta.x > swipeDetectionThreshold)
        {
            if (mouseWorldPosition.y < cursorPosition.y)
            {
                HexagonManager.Instance.RotateHexagons(false);
            }
            else
            {
                HexagonManager.Instance.RotateHexagons(true);
            }
        }
        else if (mouseDelta.x < -swipeDetectionThreshold)
        {
            if (mouseWorldPosition.y < cursorPosition.y)
            {
                HexagonManager.Instance.RotateHexagons(true);
            }
            else
            {
                HexagonManager.Instance.RotateHexagons(false);
            }
        }
    }

    /// <summary>
    /// Converts mouse position to world position
    /// </summary>
    /// <returns>Mouse position in world</returns>
    private Vector3 MousePositionToWorldPosition()
    {
        var point = gameCamera.ScreenToWorldPoint(Input.mousePosition);
        point.z = 0;
        return point;
    }

    private void ChangeCursorPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = 0;
        mousePosition = gameCamera.ScreenToWorldPoint(mousePosition);

        CursorManager.Instance.MoveCursorToNearestSnapPoint(mousePosition);
    }

    /// <summary>
    /// Resets the manager
    /// </summary>
    public void ResetManager()
    {
        _initialTouchPosition = Vector3.zero;
        
        _enableInput = false;
        _cancelNextMove = false;
    }
}