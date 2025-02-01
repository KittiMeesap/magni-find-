using UnityEngine;

public class PhotoButton : MonoBehaviour
{
    public enum ButtonType { Left, Right }
    public ButtonType buttonType;

    private void OnMouseDown()
    {
        if(ToolManager.Instance.CurrentMode == "Hand")
            {
            if (buttonType == ButtonType.Left)
            {
                CameraMinigame.Instance.PreviousPhoto();
            }
            else if (buttonType == ButtonType.Right)
            {
                CameraMinigame.Instance.NextPhoto();
            }
        }
        
    }
}
