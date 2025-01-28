using UnityEngine;
using System.Collections;

public class InteractableObject : MonoBehaviour
{
    [Header("On/Off")]
    [SerializeField] private bool canBreak = true; // กำหนดวัตถุนี้แตกได้หรือไม่
    [SerializeField] private bool canScale = true;

    [Header("Scale")]
    [SerializeField] private ActionType requiredAction; // การกระทำที่ต้องการ (ย่อหรือขยาย)
    [SerializeField] public float maxScale = 3f; // ขนาดสูงสุดที่สามารถขยายได้
    [SerializeField] public float minScale = 0.5f; // ขนาดต่ำสุดที่สามารถย่อได้

    [Header("Reward")]
    [SerializeField] public GameObject hiddenItem; // ไอเท็มที่ซ่อนอยู

    [Header("MiniGame")]
    [SerializeField] private bool MiniGameObject = false;
    [SerializeField] private GameObject minigamePrefab;
    public int minigameNumber;

    public bool IsMiniGameObject => MiniGameObject;
    public GameObject MinigamePrefab => minigamePrefab;


    private bool isActionComplete = false; // เช็คว่าการกระทำเสร็จสมบูรณ์แล้วหรือไม่

    private enum ActionType { Expand, Shrink } // ประเภทการกระทำ

    public void ModifyScale(float scaleChange)
    {
        if (canScale)
        {
            // เช็คว่าวัตถุนี้เป็นวัตถุที่ถูกเลือกหรือไม่
            if (ToolManager.Instance.GetSelectedObject() != gameObject)
            {
                Debug.LogWarning($"{gameObject.name} is not the currently selected object!");
                return;
            }

            // หากการกระทำเสร็จสมบูรณ์แล้ว ไม่ทำอะไรต่อ
            if (isActionComplete) return;

            // ปรับขนาดวัตถุ
            Vector3 newScale = transform.localScale + Vector3.one * scaleChange;

            // จำกัดขนาดให้อยู่ในช่วง maxScale และ minScale
            if (newScale.x > maxScale) newScale = Vector3.one * maxScale;
            if (newScale.x < minScale) newScale = Vector3.one * minScale;

            transform.localScale = newScale;

            // ตรวจสอบการกระทำ
            if (requiredAction == ActionType.Expand) // หากต้องการ "ขยาย"
            {
                if (scaleChange > 0 && newScale.x >= maxScale)
                {
                    RevealItem(); // การกระทำถูกต้อง
                }
                else if (scaleChange < 0 && newScale.x <= minScale && canBreak)
                {
                    HandleWrongAction(); // การกระทำผิด
                }
            }
            else if (requiredAction == ActionType.Shrink) // หากต้องการ "ย่อ"
            {
                if (scaleChange < 0 && newScale.x <= minScale)
                {
                    RevealItem(); // การกระทำถูกต้อง
                }
                else if (scaleChange > 0 && newScale.x >= maxScale && canBreak)
                {
                    HandleWrongAction(); // การกระทำผิด
                }
            }
        }
    }

    private void RevealItem()
    {
        if (canBreak)
        {
            isActionComplete = true;
            Debug.Log($"Correct action! {gameObject.name} revealed an item.");
            if (hiddenItem != null)
            {
                hiddenItem.SetActive(true);
            }
            Destroy(gameObject); // ลบวัตถุที่แตก
        }
    }


    private void HandleWrongAction()
    {
        if (canBreak)
        {
            isActionComplete = true;
            Debug.Log($"Wrong action! {gameObject.name} destroyed and item lost.");
            if (hiddenItem != null)
            {
                Destroy(hiddenItem); // ลบไอเท็มที่ซ่อนหากทำผิด
            }
            Destroy(gameObject); // ลบวัตถุที่แตก
        }
    }

    public void ExitMiniGame()
    {
        if (MiniGameObject)
        {
            Debug.Log($"Exiting mini-game for {gameObject.name}");
            if (minigamePrefab != null)
            {
                Destroy(minigamePrefab); // ปิดมินิเกม
                minigamePrefab = null; // ล้างอ้างอิง
            }

            // เพิ่มการรีเซ็ตสถานะถ้าจำเป็น
            ResetMiniGameState();
        }
    }

    private void ResetMiniGameState()
    {
        isActionComplete = false;
        Debug.Log($"Mini-game for {gameObject.name} has been reset.");
    }
}