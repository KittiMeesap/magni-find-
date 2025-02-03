using UnityEngine;

public class PlayMinigame : MonoBehaviour
{
    public static PlayMinigame Instance { get; private set; }

    private int totalParts = 4;
    private int completedParts = 0;
    private int savedCompletedParts = 0;

    [SerializeField] private Transform playTransform;
    [SerializeField] private float snapDistance = 0.5f;
    [SerializeField] private Vector3 correctScale;

    [SerializeField] private Transform pTransform;
    [SerializeField] private Transform lTransform;
    [SerializeField] private Transform aTransform;
    [SerializeField] private Transform yTransform;

    [SerializeField] private Transform pShadowTransform;
    [SerializeField] private Transform lShadowTransform;
    [SerializeField] private Transform aShadowTransform;
    [SerializeField] private Transform yShadowTransform;

    private bool isMinigameCompleted = false;
    private Transform draggingLetter = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        HandleDragging();

        if (isMinigameCompleted && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Starting the next level...");
            MainMenuManager.Instance.LoadNextLevel(); // Load next level
        }
    }

    private Transform GetShadowForLetter(InteractableObjectPlayMinigame letter)
    {
        if (letter.name.StartsWith("P")) return pShadowTransform;
        if (letter.name.StartsWith("L")) return lShadowTransform;
        if (letter.name.StartsWith("A")) return aShadowTransform;
        if (letter.name.StartsWith("Y")) return yShadowTransform;

        return null; // If no matching letter
    }

    private void HandleDragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Check if clicked on a letter
            draggingLetter = GetLetterUnderMouse();
        }
        else if (Input.GetMouseButton(0) && draggingLetter != null)
        {
            // Drag the letter if selected
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            draggingLetter.position = mousePos;
        }
        else if (Input.GetMouseButtonUp(0) && draggingLetter != null)
        {
            // Check snap on mouse release
            CheckForSnap(draggingLetter);
            draggingLetter = null;
        }
    }

    private Transform GetLetterUnderMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        if (hit != null)
        {
            if (hit.transform == pTransform || hit.transform == lTransform || hit.transform == aTransform || hit.transform == yTransform)
            {
                return hit.transform;
            }
        }

        return null;
    }

    private void CheckForSnap(Transform letter)
    {
        Transform shadowTarget = null;

        // Map the letter to its corresponding shadow target
        if (letter == pTransform) shadowTarget = pShadowTransform;
        else if (letter == lTransform) shadowTarget = lShadowTransform;
        else if (letter == aTransform) shadowTarget = aShadowTransform;
        else if (letter == yTransform) shadowTarget = yShadowTransform;

        // If shadow target exists
        if (shadowTarget != null)
        {
            // Check if the letter is close enough to its shadow
            if (Vector3.Distance(letter.position, shadowTarget.position) <= snapDistance)
            {
                // Only snap if the scale is correct
                if (letter.localScale == correctScale)
                {
                    letter.position = shadowTarget.position; // Snap to shadow position
                    CompletePart(); // Mark the part as completed
                }
                else
                {
                    Debug.Log("The scale is incorrect. Cannot snap.");
                }
            }
            else
            {
                Debug.Log("Letter is not close enough to snap.");
            }
        }
    }

    public void CheckLetterPlacement(InteractableObjectPlayMinigame letter)
    {
        // Check if the letter is near its shadow
        float snapDistance = 50f; // Set the snap distance threshold
        Transform shadowTransform = GetShadowForLetter(letter);

        if (shadowTransform != null && Vector2.Distance(letter.transform.position, shadowTransform.position) <= snapDistance)
        {
            // Only snap if scale matches the correct scale
            if (letter.transform.localScale == correctScale)
            {
                letter.transform.position = shadowTransform.position; // Snap to shadow
                CompletePart(); // Mark this part as completed
            }
            else
            {
                Debug.Log("The scale is incorrect. Cannot snap.");
            }
        }
    }

    public void CompletePart()
    {
        completedParts++;
        savedCompletedParts = completedParts;
        Debug.Log($"Play Part completed: {completedParts}/{totalParts}");

        if (completedParts >= totalParts)
        {
            isMinigameCompleted = true;
            Debug.Log("All letters placed correctly. Press Spacebar to continue.");
            playTransform.gameObject.SetActive(true); // Activate the completion screen
        }
    }
}
