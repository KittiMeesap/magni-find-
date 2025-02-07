using UnityEngine;

public class PowerButton : MonoBehaviour
{
    private bool isOn = false;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnMouseDown()
    {
        if (ToolManager.Instance.CurrentMode != "Hand") return; // ✅ ต้องอยู่ในโหมด Hand เท่านั้น
        if (!TurntableMinigame.Instance.CanPressPower) return; // ✅ ถ้าไม่มีแผ่นเสียงห้ามเปิด

        isOn = !isOn;
        spriteRenderer.sprite = isOn ? onSprite : offSprite;

        if (isOn)
        {
            TurntableMinigame.Instance.TurnOn();
        }
        else
        {
            TurntableMinigame.Instance.TurnOff();
        }
    }
}
