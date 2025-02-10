using UnityEngine;

public class ChangePictureAfterReward : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetSpriteRenderer; // ✅ ตัว SpriteRenderer ของ Object ที่จะเปลี่ยน
    [SerializeField] private Sprite newSprite; // ✅ Sprite ที่จะเปลี่ยนเป็นหลังจากกด

    private bool isChanged = false; // ✅ เช็คว่าเปลี่ยนไปแล้วหรือยัง

    private void OnMouseDown()
    {
        if (!isChanged && targetSpriteRenderer != null && newSprite != null)
        {
            targetSpriteRenderer.sprite = newSprite; // ✅ เปลี่ยน Sprite
            isChanged = true; // ✅ ป้องกันการเปลี่ยนซ้ำ
        }
    }
}
