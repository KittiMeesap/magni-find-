using UnityEngine;
using System.Collections;
using SpriteGlow;

public class InteractObject : MonoBehaviour
{
    [SerializeField] private MinigameType minigameType;
    public enum MinigameType
    {
        Null,
        Clock,
        Vase,
        Camera,
        Cat,
        Picture,
        Instrument,
        Bird
    }

    private SpriteGlowEffect glowEffect;
    private bool isHovering = false;
    private float oscillationSpeed = 3f; // ความเร็วในการแกว่ง
    private float oscillationAmount = 0.05f; // ขนาดของการแกว่ง
    private Vector3 initialPosition; // ตำแหน่งเริ่มต้นของ Object

    private void Awake()
    {
        glowEffect = GetComponent<SpriteGlowEffect>();
        initialPosition = transform.position;
    }

    private void OnMouseOver()
    {
        if (MinigameManager.Instance.IsPlayingMinigame == false && ToolManager.Instance.CurrentMode == "Eye")
        {
            if (glowEffect != null)
            {
                glowEffect.enabled = true; // เปิดเอฟเฟกต์ Glow
            }

            if (!isHovering)
            {
                isHovering = true;
                StartCoroutine(SwingObject());
            }
            
        }
        else
        {
            if (glowEffect != null)
            {
                glowEffect.enabled = false; // ปิดเอฟเฟกต์ Glow
            }

            isHovering = false;
            StopCoroutine(SwingObject());
            transform.position = initialPosition; // คืนค่าเป็นตำแหน่งเริ่มต้น
        }
    }

    private void OnMouseExit()
    {
        if (glowEffect != null)
        {
            glowEffect.enabled = false; // ปิดเอฟเฟกต์ Glow
        }

        isHovering = false;
        StopCoroutine(SwingObject());
        transform.position = initialPosition; // คืนค่าเป็นตำแหน่งเริ่มต้น
    }

    private IEnumerator SwingObject()
    {
        float elapsedTime = 0f;

        while (isHovering)
        {
            elapsedTime += Time.deltaTime * oscillationSpeed;
            float offset = Mathf.Sin(elapsedTime) * oscillationAmount;
            transform.position = new Vector3(initialPosition.x + offset, initialPosition.y, initialPosition.z);

            yield return null; // รอเฟรมถัดไป
        }
    }


private void OnMouseDown()
    {
        if (MinigameManager.Instance.IsPlayingMinigame == false && ToolManager.Instance.CurrentMode == "Eye")
        {
            ToolManager.Instance.SetToolMode("Hand");

            DialogueSystem dialogueSystem = GetComponent<DialogueSystem>();
            if (dialogueSystem != null)
            {
                dialogueSystem.ShowDialogue();
            }
            if (minigameType != MinigameType.Null)
            {
                StartMinigame();
            }
        }
    }

    private void StartMinigame()
    {
        switch (minigameType)
        {
            case MinigameType.Clock:
                ClockMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Vase:
                VaseMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Camera:
                CameraMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Cat:
                CatMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Picture:
                PictureMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Instrument:
                // InstrumentMinigame.Instance.StartMinigame();
                break;
            case MinigameType.Bird:
                // BirdMinigame.Instance.StartMinigame();
                break;
            default:
                Debug.LogWarning("Not Minigame Object");
                break;
        }
    }
}
