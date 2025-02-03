using UnityEngine;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [SerializeField] private GameObject playMinigameObject;
    [SerializeField] private Transform playTransform;
    [SerializeField] private GameObject triggerObject;

    [SerializeField] private Transform gameTitleTransform;  // ชื่อตัวเกมที่จะเคลื่อนที่
    [SerializeField] private Transform targetPosition;  // ตำแหน่งเป้าหมายที่ต้องการให้ชื่อเกมไปถึง

    [SerializeField] private float animationDuration = 1f;  // ระยะเวลาของการเคลื่อนที่

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

        // เริ่มซ่อน Minigame play ก่อน
        playMinigameObject.SetActive(false);
    }

    public void StartMinigame()
    {
        // เริ่มมินิเกมโดยการเคลื่อนที่ชื่อเกม
        StartCoroutine(MoveGameTitleToTarget());
        Debug.Log("Minigame started from MainMenuManager!");
    }

    private IEnumerator MoveGameTitleToTarget()
    {
        Vector3 initialPosition = gameTitleTransform.position; // ตำแหน่งเริ่มต้นของชื่อเกม
        float elapsedTime = 0f;

        // เคลื่อนที่ชื่อเกมไปที่ตำแหน่งที่ตั้งไว้
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / animationDuration;
            gameTitleTransform.position = Vector3.Lerp(initialPosition, targetPosition.position, t); // เคลื่อนที่
            yield return null;
        }

        // เมื่อชื่อเกมถึงตำแหน่งแล้ว
        gameTitleTransform.position = targetPosition.position;

        // แสดง Minigame play
        playMinigameObject.SetActive(true);
        playTransform.gameObject.SetActive(true);

        Debug.Log("Minigame play activated!");
    }

    public void SetMinigameTrigger(GameObject trigger)
    {
        triggerObject = trigger;
        Debug.Log("Minigame trigger set.");
    }
}
