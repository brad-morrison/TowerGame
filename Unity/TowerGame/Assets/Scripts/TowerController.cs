using UnityEngine;

public class TowerController : MonoBehaviour
{
    public GameManager gameManager;
    [Header("Spiral Segment Settings")]
    [Tooltip("Height of a single wedge segment in units")]
    public float segmentHeight = 0.4f;

    [Tooltip("Rotation angle per wedge segment in degrees")]
    public float anglePerSegment = 7.5f;

    [Header("Movement")]
    [Tooltip("How fast the tower rotates in degrees/second")]
    public float rotationSpeed = 90f;

    private float scrollSpeed => GetScrollSpeed(); // Always up-to-date

    void Update()
    {
        if (gameManager.gameActive)
        {
            // Apply rotation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Apply downward scroll to simulate ascent
            transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime, Space.World);
        }
        
    }

    private float GetScrollSpeed()
    {
        float segmentsPerCircle = 360f / anglePerSegment;
        float heightPerCircle = segmentsPerCircle * segmentHeight;
        return rotationSpeed / 360f * heightPerCircle;
    }
}