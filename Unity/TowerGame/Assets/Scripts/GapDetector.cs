using UnityEngine;

public class GapDetector : MonoBehaviour
{
    public LayerMask rampLayer;
    public float checkDistance = 1f;

    private BoxCollider box;

    void Start()
    {
        box = GetComponent<BoxCollider>();
    }

    void Update()
    {
        Ray ray = new Ray(transform.parent.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, checkDistance, rampLayer))
        {
            box.enabled = true; // Ramp under player
        }
        else
        {
            box.enabled = false; // No ramp — player is over a gap
        }
    }
}