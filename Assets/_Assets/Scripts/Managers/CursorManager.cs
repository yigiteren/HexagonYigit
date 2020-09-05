using UnityEngine;

public class CursorManager : MonoBehaviour
{
    // Properties //
    public static CursorManager Instance { get; private set; }
    public CursorController Cursor { get; private set; }
    // Editor Variables //
    [SerializeField] private GameObject cursorPrefab;
    
    // Private Variables //
    private GameObject _cursor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    /// <summary>
    /// Spawns the initial cursor.
    /// </summary>
    public void SpawnCursor()
    {
        if (_cursor == null)
        {
            _cursor = Instantiate(cursorPrefab, Vector2.zero, Quaternion.identity);
            Cursor = _cursor.GetComponent<CursorController>();

            _cursor.SetActive(false);
        }
    }

    /// <summary>
    /// Moves cursor to nearest snap point to "position" parameter.
    /// </summary>
    /// <param name="position">Position to move</param>
    public void MoveCursorToNearestSnapPoint(Vector2 position)
    {
        _cursor.SetActive(true);
        
        var snapPoint = GridManager.Instance.FindNearestCursorSnapPoint(position);
        _cursor.transform.position = new Vector3(snapPoint.x, snapPoint.y, -0.3f);
    }
}