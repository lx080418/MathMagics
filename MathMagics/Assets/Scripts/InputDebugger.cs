using UnityEngine;

/// <summary>
/// A helper script that logs any key or mouse input to the console.
/// Useful for debugging and verifying input mappings.
/// Attach this to any active GameObject in your scene.
/// </summary>
public class InputDebugger : MonoBehaviour
{
    void Update()
    {
        // Log any key that was pressed this frame
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                Debug.Log($"Key Pressed: {key}");
            }
        }

        // Optionally, log mouse button presses
        for (int i = 0; i <= 2; i++) // Left(0), Right(1), Middle(2)
        {
            if (Input.GetMouseButtonDown(i))
            {
                Debug.Log($"Mouse Button Pressed: {i}");
            }
        }
    }
}
