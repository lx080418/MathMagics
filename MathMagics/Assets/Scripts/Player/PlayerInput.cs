using UnityEngine;

/// <summary>
/// Captures player input and raises movement/attack events.
/// Does NOT directly move the player — it delegates.
/// </summary>
public class PlayerInput : MonoBehaviour
{
    public static Vector2 lastDirection = Vector2.right;

    // Events for movement and attack
    public static event System.Action<Vector2> OnMoveInput;
    public static event System.Action OnSpaceInput;
    public static bool canMove = true;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) TriggerMove(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) TriggerMove(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) TriggerMove(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) TriggerMove(Vector2.right);
        else if (Input.GetKeyDown(KeyCode.Space)) TriggerSpacePressed();
    }

    private void TriggerMove(Vector2 direction)
    {
        if(!canMove) return;
        Debug.Log($"Move input received: {direction}");
        lastDirection = direction;
        OnMoveInput?.Invoke(direction);
    }

    private void TriggerSpacePressed()
    {
        OnSpaceInput?.Invoke();
    }

    public static void LockMovement()
    {
        canMove = false;
    }


    public static void UnlockMovement()
    {
        canMove = true;
    }
    

    
}
