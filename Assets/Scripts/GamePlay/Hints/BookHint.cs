using UnityEngine;

public class BookHint : MonoBehaviour
{
    public static BookHint Instance { get; private set; }

    [SerializeField] private GameObject bookObject; // ตัวแสดง Hint (Book)
    [SerializeField] private DialogueManager targetDialogueSystem; // Dialogue ที่จะใช้
    [SerializeField] private string message; // ข้อความใน Dialogue

    private bool isHintActive = false; // ตรวจสอบสถานะการแสดงผล Hint

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

    // เริ่มแสดง Hint และเปิด Dialogue
    public void StartHint()
    {
        if (targetDialogueSystem != null)
        {
            targetDialogueSystem.ShowDialogue(message); // แสดงข้อความในไดอาล็อค
        }

        // แสดง Hint (Book)
        bookObject.SetActive(true);
        isHintActive = true;
    }

    // ปิด Hint และ Dialogue
    public void EndHint()
    {
        if (targetDialogueSystem != null)
        {
            targetDialogueSystem.HideDialogue(); // ปิด Dialogue
        }

        // ซ่อน Hint (Book)
        bookObject.SetActive(false);
        isHintActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isHintActive)
        {
            StartHint(); // เริ่มระบบ Hint เมื่อผู้เล่นเข้าใกล้
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isHintActive)
        {
            EndHint(); // ปิดระบบ Hint เมื่อผู้เล่นออกจากพื้นที่
        }
    }
}
