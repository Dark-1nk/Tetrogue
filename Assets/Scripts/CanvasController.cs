using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    public Canvas targetCanvas; // Reference to the Canvas to show/hide
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    // Function to activate the Canvas
    public void ShowCanvas()
    {
        if (targetCanvas != null)
        {
            audioManager.PlaySFX(audioManager.collect);
            targetCanvas.gameObject.SetActive(true);
        }
    }

    // Function to deactivate the Canvas
    public void HideCanvas()
    {
        if (targetCanvas != null)
        {
            audioManager.PlaySFX(audioManager.placeBlock);
            targetCanvas.gameObject.SetActive(false);
        }
    }
}
