using UnityEngine;
using System.Collections;

public class InteractableBird : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite readyToEatSprite;
    [SerializeField] private Sprite walkingSprite;
    [SerializeField] private Sprite eatingSprite;
    private Collider2D birdCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        birdCollider = GetComponent<Collider2D>();
    }

    public void SetBirdState(string state)
    {
        switch (state)
        {
            case "idle":
                spriteRenderer.sprite = idleSprite;
                break;
            case "readyToEat":
                spriteRenderer.sprite = readyToEatSprite;
                break;
            case "walking":
                spriteRenderer.sprite = walkingSprite;
                break;
            case "eating":
                spriteRenderer.sprite = eatingSprite;
                StartCoroutine(DisableColliderAfterEating());
                break;
        }
    }

    private IEnumerator DisableColliderAfterEating()
    {
        yield return new WaitForSeconds(1f); // ✅ รอให้กินเสร็จก่อน
        if (birdCollider != null)
        {
            birdCollider.enabled = false; // ✅ ปิด Collider ของนกหลังจากกินเสร็จ
        }
    }
}
