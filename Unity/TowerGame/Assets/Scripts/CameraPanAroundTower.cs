using UnityEngine;

/// <summary>
/// Gently pans the camera left/right around the tower axis with a sine ease.
/// Attach to a camera pivot (recommended) or the camera itself.
/// </summary>
public class CameraPanAroundTower : MonoBehaviour
{
    public GameManager gameManager;
    public Transform towerCenter;

    public float amplitudeDeg = 10f;
    public float periodSeconds = 8f;
    public float phaseDeg = 0f;

    public Mode panMode = Mode.YawInPlace;
    public enum Mode { YawInPlace, OrbitAroundCenter }

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

        if (lookTarget)
        {
            var pos = lookTarget.position;
            pos.y = Mathf.Lerp(transform.position.y, pos.y, 0.0f);
            transform.LookAt(pos, Vector3.up);
        }
    }
}
