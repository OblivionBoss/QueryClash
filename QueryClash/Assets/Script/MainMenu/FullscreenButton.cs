using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FullscreenButton : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public Button fullscreenButton;         // Reference to the button
    public Image buttonImage;               // Reference to the button's image
    public Sprite fullscreenSprite;        // Sprite for fullscreen mode
    public Sprite windowedSprite;          // Sprite for windowed mode
    //public Sprite hoverSprite;             // Optional: Sprite when hovering (if different)
    //public TMP_Text buttonText;                 // Reference to the button's text component
    //public Color fullscreenColor = Color.white;      // Color in fullscreen mode
    //public Color windowedColor = Color.black;        // Color in windowed mode
    //public Color fullscreenHoverColor = Color.black; // Hover color in fullscreen mode
    //public Color windowedHoverColor = Color.white;   // Hover color in windowed mode

    private bool isFullscreen = true;       // Track fullscreen state
    //private bool isHovering = false;        // Track if hovering

    void Start()
    {
        // Ensure the app starts in fullscreen mode by default
        Screen.fullScreen = true;
        UpdateButtonAppearance();
        //UpdateButtonText();                 // Update the text on start

        // Add listener to the button's onClick event
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
    }

    void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;       // Toggle fullscreen state
        Screen.fullScreen = isFullscreen;   // Update screen mode
        UpdateButtonAppearance();           // Update button appearance
        //UpdateButtonText();                 // Update button text
    }

    void UpdateButtonAppearance()
    {
        buttonImage.sprite = isFullscreen ? fullscreenSprite : windowedSprite;
        // Update the button's sprite based on fullscreen state and hover state
        //if (isHovering && hoverSprite != null) buttonImage.sprite = hoverSprite; // Use hover sprite if hovering
        //else buttonImage.sprite = isFullscreen ? fullscreenSprite : windowedSprite;
    }
    //void UpdateButtonAppearance()
    //{
    //    // Determine the button color based on fullscreen state and hover state
    //    if (isFullscreen) { buttonImage.color = isHovering ? fullscreenHoverColor : fullscreenColor; }
    //    else { buttonImage.color = isHovering ? windowedHoverColor : windowedColor; }
    //}

    //void UpdateButtonText()
    //{
    //    // Update the text based on fullscreen state
    //    //buttonText.text = isFullscreen ? "FULLSCREEN : ON" : "FULLSCREEN : OFF";
    //}

    //// Event when pointer enters the button
    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    isHovering = true;
    //    UpdateButtonAppearance();
    //}

    //// Event when pointer exits the button
    //public void OnPointerExit(PointerEventData eventData)
    //{
    //    isHovering = false;
    //    UpdateButtonAppearance();
    //}
}
