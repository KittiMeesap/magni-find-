﻿using UnityEngine;

public class VinylDisc : MonoBehaviour
{
    public static VinylDisc Instance { get; private set; }
    private bool isDragging = false;
    private Vector3 offset;
    [SerializeField] private Transform trayPosition; // ✅ จุดที่แผ่นเสียงจะไปวาง
    public Transform TrayPosition => trayPosition;

    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition; // ✅ เก็บตำแหน่งก่อนลาก

    [SerializeField] private Sprite defaultSprite;       // ✅ Sprite ปกติ
    [SerializeField] private Sprite highlightedSprite;   // ✅ Sprite เมื่อเอาเมาส์ไปวาง

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer != null)
        {
            defaultSprite = spriteRenderer.sprite;
        }

        originalPosition = transform.position; // ✅ บันทึกตำแหน่งเริ่มต้น
    }

    private void OnMouseDown()
    {
        if (Time.timeScale == 0f) return;
        if (!TurntableMinigame.Instance.CanInsertVinyl) return; // ✅ ป้องกันลากเข้าไปซ้ำ

        SoundManager.Instance.PlaySFX(SoundManager.Instance.sfx_Hand);
        isDragging = true;
        offset = transform.position - GetMouseWorldPos();
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // ✅ ถ้าห่างจาก trayPosition เกิน 1.0f ให้กลับไปที่ตำแหน่งเดิมทันที
        if (Vector3.Distance(transform.position, trayPosition.position) < 1.0f)
        {
            transform.position = trayPosition.position;
            TurntableMinigame.Instance.InsertVinyl();
        }
        else
        {
            transform.position = originalPosition; // ✅ กลับไปที่ตำแหน่งเดิมทันที
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseOver()
    {
        if (Time.timeScale == 0f) return;
        if (transform.position != trayPosition.position)
        {
            if (MinigameManager.Instance.IsPlayingMinigame && ToolManager.Instance.CurrentMode == "Hand")
            {
                if (spriteRenderer != null && highlightedSprite != null)
                {
                    spriteRenderer.sprite = highlightedSprite;
                }
            }
            else
            {
                if (spriteRenderer != null && defaultSprite != null)
                {
                    spriteRenderer.sprite = defaultSprite;
                }
            }
        }
        else
        {
            if (spriteRenderer != null && defaultSprite != null)
            {
                spriteRenderer.sprite = defaultSprite;
            }
        }
    }

    private void OnMouseExit()
    {
        if (spriteRenderer != null && defaultSprite != null)
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
