using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset = new Vector3(0, 5, -10);
    public float followSpeed = 2f;

    void LateUpdate()
    {
        if (!player) return;

        Vector3 targetPosition = new Vector3(transform.position.x, player.position.y + offset.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);
    }
}