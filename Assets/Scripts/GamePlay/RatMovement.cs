using UnityEngine;
using System.Collections;

public class RatMovement : MonoBehaviour
{
    public Transform[] waypoints; // จุด Waypoints ที่หนูจะเดินผ่าน
    public float moveSpeed = 2.0f; // ความเร็วในการเคลื่อนที่

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    public void StartMoving()
    {
        if (!isMoving && waypoints.Length > 0)
        {
            StartCoroutine(MoveThroughWaypoints());
        }
    }

    private IEnumerator MoveThroughWaypoints()
    {
        isMoving = true;

        while (currentWaypointIndex < waypoints.Length)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = waypoints[currentWaypointIndex].position;
            float elapsedTime = 0f;

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * moveSpeed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
                yield return null;
            }

            transform.position = targetPosition;
            currentWaypointIndex++;
        }

        isMoving = false;
        Debug.Log("Rat has reached its final destination.");
    }
}
