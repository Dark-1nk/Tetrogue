using UnityEngine;

public class Goal : MonoBehaviour
{
    // Reference to the PlayerController script
    public CharacterControllerScript characterController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Notify the PlayerController of the victory
            if (characterController != null)
            {
                characterController.Victory();
            }
        }
    }
}
