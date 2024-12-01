using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Function to quit the game
    public void Quit()
    {
        // Closes the application
        Application.Quit();

        // Log message for debugging (will only appear in the Unity Editor)
        Debug.Log("Game is quitting...");
    }
}
