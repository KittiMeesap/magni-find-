using UnityEngine;

public class PhotoButton : MonoBehaviour
{
    public enum ButtonType { Left, Right }
    public ButtonType buttonType;

    private SpriteRenderer spriteRenderer;
    private Sprite defaultSprite;
    [SerializeField] private Sprite highlightedSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode == "Hand")
        {
            bool canGoLeft = buttonType == ButtonType.Left && CameraMinigame.Instance.CurrentPhotoIndex > 0;
            bool canGoRight = buttonType == ButtonType.Right && CameraMinigame.Instance.CurrentPhotoIndex < CameraMinigame.Instance.Photos.Count - 1;

            if (canGoLeft)
            {
                CameraMinigame.Instance.PreviousPhoto();
            }
            else if (canGoRight)
            {
                CameraMinigame.Instance.NextPhoto();
            }
        }
    }


    private void OnMouseOver()
    {
        if (MinigameManager.Instance.IsPlayingMinigame && (ToolManager.Instance.CurrentMode == "Hand"))
        {
            bool isLeftActive = buttonType == ButtonType.Left && CameraMinigame.Instance.Photos.Count > 0 && CameraMinigame.Instance.CurrentPhotoIndex > 0;
            bool isRightActive = buttonType == ButtonType.Right && CameraMinigame.Instance.Photos.Count > 0 && CameraMinigame.Instance.CurrentPhotoIndex < CameraMinigame.Instance.Photos.Count - 1;

            if ((isLeftActive || isRightActive) && spriteRenderer != null && highlightedSprite != null)
            {
                spriteRenderer.sprite = highlightedSprite;
            }
            else
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
        else
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
