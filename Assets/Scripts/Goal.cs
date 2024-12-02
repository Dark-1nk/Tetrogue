using UnityEngine;

public class Goal : MonoBehaviour
{
    // Reference to the PlayerController script
    public CharacterControllerScript characterController;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            characterController.Victory();
        }
    }
}
