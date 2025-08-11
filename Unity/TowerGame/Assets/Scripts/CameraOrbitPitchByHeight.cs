using UnityEngine;

public class CameraOrbitPitchByHeight : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;
    [Tooltip("If set, yaw will face outward from this center (tower). Leave null to use a fixed yaw.")]
    public Transform towerCenter;

    [Header("Orbit")]
    public float distance = 12f;         // how far the camera sits from the player
    public float fixedYawDeg = 0f;       // used if towerCenter is not set
    public float positionSmooth = 6f;    // higher = snappier

    [Header("Pitch by Height")]
    public float startPitchDeg = 25f;    // looking down near ground
    public float endPitchDeg = -8f;      // slight look up high in the tower
    public float startHeight = 0f;       // when the pitch starts changing
    public float endHeight = 50f;        // when the pitch finishes changing
    public float pitchSmooth = 8f;       // smoothing for pitch changes

    [Header("Orthographic (optional)")]
    public Camera cam;                   // assign your Camera if you want size control
    public bool scaleOrthoSize = false;  // e.g., zoom out as player climbs
    public float orthoStartSize = 6f;
    public float orthoEndSize = 8.5f;

    float _pitchVel;     // for SmoothDampAngle
    float _currPitchDeg; // smoothed pitch

    void Awake()
    {
        if (!cam) cam = GetComponent<Camera>();
        _currPitchDeg = startPitchDeg;
        if (cam && cam.orthographic && scaleOrthoSize) cam.orthographicSize = orthoStartSize;
    }

    void LateUpdate()
    {
        if (!player) return;

        // Map player height -> 0..1
        float t = Mathf.InverseLerp(startHeight, endHeight, player.position.y);
        t = Mathf.Clamp01(t);

        // Smooth pitch towards target based on height
        float targetPitch = Mathf.Lerp(startPitchDeg, endPitchDeg, t);
        _currPitchDeg = Mathf.SmoothDampAngle(_currPitchDeg, targetPitch, ref _pitchVel, 1f / Mathf.Max(0.0001f, pitchSmooth));

        // Yaw: either face outward from tower, or use fixed yaw
        float yawDeg = fixedYawDeg;
        if (towerCenter)
        {
            Vector3 radial = player.position - towerCenter.position;
            radial.y = 0f;
            if (radial.sqrMagnitude > 0.0001f)
            {
                yawDeg = Mathf.Atan2(radial.x, radial.z) * Mathf.Rad2Deg; // outward from center
            }
        }

        // Desired camera rotation and position along a vertical arc around the player
        Quaternion rot = Quaternion.Euler(_currPitchDeg, yawDeg, 0f);
        Vector3 desiredPos = player.position - (rot * Vector3.forward * distance);

        // Smooth move & look at player (works fine in orthographic)
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionSmooth);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(player.position - transform.position, Vector3.up), Time.deltaTime * positionSmooth);

        // Optional: adjust orthographic size by height
        if (cam && cam.orthographic && scaleOrthoSize)
        {
            float targetSize = Mathf.Lerp(orthoStartSize, orthoEndSize, t);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * positionSmooth);
        }
    }
}
