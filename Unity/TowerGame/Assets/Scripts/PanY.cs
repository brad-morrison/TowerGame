using UnityEngine;

public class MoveDownY : MonoBehaviour
{
    [Tooltip("Units per second downward (world Y).")]
    public float speed = 2f;

    [Tooltip("Stop at this world Y (optional).")]
    public bool useMinY = false;
    public float minY = -10f;

    void Update()
    {
        // Move down in world space
        transform.Translate(Vector3.down * speed * Time.deltaTime, Space.World);

        if (useMinY && transform.position.y < minY)
        {
            var p = transform.position;
            p.y = minY;
            transform.position = p;
            enabled = false; // hit floor, stop moving
        }
    }
}