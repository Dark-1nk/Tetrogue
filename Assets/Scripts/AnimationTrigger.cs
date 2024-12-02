using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationTrigger : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        // Get the Animator component on the GameObject
        animator = GetComponent<Animator>();

        // Check if an Animator is attached to this GameObject
        if (animator == null)
        {
            Debug.LogError("Animator component not found on " + gameObject.name);
        }
    }

    void OnEnable()
    {
        // Add a listener to handle when the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Remove the listener when the object is disabled to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level 4")
        {
            // Trigger the animation on scene load
            animator.SetTrigger("Dance");
        }

        if (scene.name == "Main Menu")
        {
            animator.SetTrigger("Idle");
        }
    }
}
