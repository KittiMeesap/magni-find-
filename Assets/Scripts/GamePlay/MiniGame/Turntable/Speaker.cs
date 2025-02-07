using UnityEngine;

public class Speaker : MonoBehaviour
{
    public static Speaker Instance { get; private set; }

    [SerializeField] private Vector3 minScale = new Vector3(0.8f, 0.8f, 0.8f);
    [SerializeField] private Vector3 maxScale = new Vector3(1.2f, 1.2f, 1.2f);
    [SerializeField] private GameObject soundSymbol;

    private int currentLevel = 1;
    private int minLevel = 1;
    private int maxLevel = 5;
    private bool canScale = false;
    private bool isMouseOver = false; // ✅ ตรวจสอบว่าเมาส์อยู่บนลำโพงหรือไม่

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!canScale || !isMouseOver) return; // ✅ ต้องเปิดระบบ และเมาส์อยู่บน Object

        if (ToolManager.Instance.CurrentMode == "Magnifier") // ✅ ต้องเป็นโหมด Magnifier
        {
            if (Input.GetMouseButtonDown(0)) // ✅ คลิกซ้าย -> ขยาย
            {
                ChangeSpeakerSize(1);
            }
            else if (Input.GetMouseButtonDown(1)) // ✅ คลิกขวา -> ย่อ
            {
                ChangeSpeakerSize(-1);
            }
        }
    }

    private void ChangeSpeakerSize(int change)
    {
        currentLevel = Mathf.Clamp(currentLevel + change, minLevel, maxLevel);
        UpdateSpeakerSize();
    }

    private void UpdateSpeakerSize()
    {
        float t = (float)(currentLevel - minLevel) / (maxLevel - minLevel);
        transform.localScale = Vector3.Lerp(minScale, maxScale, t);

        // ✅ ปรับ Alpha ของ Sound Symbol ตามระดับเสียง
        float alpha = Mathf.Lerp(0f, 1f, t);
        soundSymbol.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, alpha);
    }

    public void EnableScaling(bool enable)
    {
        canScale = enable;
    }

    private void OnMouseOver()
    {
        isMouseOver = true;
    }

    private void OnMouseExit()
    {
        isMouseOver = false;
    }
}
