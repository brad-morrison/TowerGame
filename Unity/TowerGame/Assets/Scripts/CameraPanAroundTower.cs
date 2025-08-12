using UnityEngine;

/// <summary>
/// Gently pans the camera left/right around the tower axis with a sine ease.
/// Attach to a camera pivot (recommended) or the camera itself.
/// </summary>
public class CameraPanAroundTower : MonoBehaviour
{
    public GameManager gameManager;
    [Header("Target / Pivot")]
    [Tooltip("Center of the pillar/tower. If null, uses world origin.")]
    public Transform towerCenter;

    [Header("Motion")]
    [Tooltip("Max pan angle each side (degrees).")]
    public float amplitudeDeg = 10f;
    [Tooltip("Time for a full left->right->left cycle (seconds).")]
    public float periodSeconds = 8f;
    [Tooltip("Start phase offset (degrees on the sine).")]
    public float phaseDeg = 0f;

    [Header("Mode")]
    public Mode panMode = Mode.YawInPlace;
    public enum Mode { YawInPlace, OrbitAroundCenter }

    [Header("Look")]
    [Tooltip("Optional: keep looking at this target (e.g., the player or tower center).")]
    public Transform lookTarget;

    Quaternion _baseLocalRot; // for YawInPlace
    float _lastAngle;         // to apply deltas without drift

    void Awake()
    {
        _baseLocalRot = transform.localRotation;
    }

    void LateUpdate()
    {
        if (periodSeconds <= 0f) return;

        if (!gameManager.gameActive) return;

        // Sine angle with natural ease-in/out
        float t = (Time.time / periodSeconds) * Mathf.PI * 2f;
        float desiredAngle = amplitudeDeg * Mathf.Sin(t + Mathf.Deg2Rad * phaseDeg);

        // Apply depending on mode
        if (panMode == Mode.YawInPlace)
        {
            transform.localRotation = _baseLocalRot * Quaternion.Euler(0f, desiredAngle, 0f);
        }
        else // OrbitAroundCenter
        {
            Vector3 center = towerCenter ? towerCenter.position : Vector3.zero;
            float delta = desiredAngle - _lastAngle;
            transform.RotateAround(center, Vector3.up, delta);
        }

        _lastAngle = desiredAngle;

        // Optional: keep the camera looking at a target (nice for orbit mode)
        if (lookTarget)
        {
            var pos = lookTarget.position;
            pos.y = Mathf.Lerp(transform.position.y, pos.y, 0.0f); // keep your current framing; tweak if needed
            transform.LookAt(pos, Vector3.up);
        }
    }
}
