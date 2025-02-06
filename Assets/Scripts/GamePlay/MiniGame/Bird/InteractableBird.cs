using UnityEngine;

public class InteractableBird : MonoBehaviour
{
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite readyToEatSprite;
    [SerializeField] private Sprite eatingSprite;

    private bool isDragging = false;
    private Vector3 originalPosition;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetBirdState("idle");
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePos;
        }
    }

    private void OnMouseDown()
    {
        if (BirdMinigame.Instance.IsAppleCorrectSize)
        {
            isDragging = true;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        float distanceToApple = Vector3.Distance(transform.position, BirdMinigame.Instance.Apple.transform.position);
        if (distanceToApple < 1.0f)
        {
            BirdMinigame.Instance.TryFeedBird();
        }
        else
        {
            transform.position = originalPosition;
        }
    }

    public void SetBirdState(string state)
    {
        if (spriteRenderer == null) return;

        switch (state)
        {
            case "idle":
                spriteRenderer.sprite = idleSprite;
                break;
            case "readyToEat":
                spriteRenderer.sprite = readyToEatSprite;
                break;
            case "eating":
                spriteRenderer.sprite = eatingSprite;
                break;
        }
    }
}
