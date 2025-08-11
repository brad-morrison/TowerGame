using UnityEngine;

public class CameraOrbitPitchByTime : MonoBehaviour
{
    [Header("Targets")]
    public Transform player;
    [Tooltip("If set, yaw will face outward from this center (e.g., tower). Leave null to use a fixed yaw.")]
    public Transform towerCenter;

    [Header("Orbit")]
    [Tooltip("How far the camera sits from the player.")]
    public float distance = 12f;
    [Tooltip("Used if towerCenter is not set.")]
    public float fixedYawDeg = 0f;
    [Tooltip("Higher = snappier movement.")]
    public float positionSmooth = 6f;

    [Header("Pitch Over Time")]
    [Tooltip("Looking down near the start.")]
    public float startPitchDeg = 25f;
    [Tooltip("Target pitch at the end (negative tilts upward).")]
    public float endPitchDeg = -8f;

    public enum CycleMode { Once, Loop, PingPong }
    [Tooltip("How the time progression behaves.")]
    public CycleMode cycleMode = CycleMode.Loop;

    [Tooltip("Seconds from start to end.")]
    public float duration = 10f;

    [Tooltip("Delay before motion starts (seconds).")]
    public float startDelay = 0f;

    [Tooltip("Use unscaled time (ignore Time.timeScale).")]
    public bool useUnscaledTime = false;

    [Tooltip("Smoothing for pitch changes (higher = snappier).")]
    public float pitchSmooth = 8f;

    [Tooltip("Map normalized time (0..1) to pitch blend (0..1).")]
    public AnimationCurve pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Yaw Motion")]
    [Tooltip("Blend the camera around the player from 'back' at start to 'front' at end.")]
    public float yawBiasStartDeg = -30f;     // start more behind the character
    public float yawBiasEndDeg   =  30f;     // end more in front of the character
    [Tooltip("Small side-to-side sway around the bias.")]
    public float yawSwayAmplitudeDeg = 20f;  // ± amount
    [Tooltip("Sway speed in cycles per second.")]
    public float yawSwaySpeedHz = 0.25f;
    [Tooltip("Extra smoothing for yaw changes (set 0 to disable).")]
    public float yawSmooth = 6f;

    [Header("Orthographic (optional)")]
    [Tooltip("Assign your Camera if you want size control. Defaults to Camera on this GameObject.")]
    public Camera cam;
    [Tooltip("If true and camera is orthographic, zoom by time.")]
    public bool scaleOrthoSize = false;
    public float orthoStartSize = 6f;
    public float orthoEndSize = 8.5f;

    float _pitchVel;       // for SmoothDampAngle
    float _currPitchDeg;   // smoothed pitch

    float _yawVel;         // for SmoothDampAngle on yaw offset
    float _currYawOffset;  // smoothed (bias + sway)

    float _startTime;      // absolute start time (with delay)

    void Awake()
    {
        if (!cam) cam = GetComponent<Camera>();
        _currPitchDeg = startPitchDeg;
        _currYawOffset = yawBiasStartDeg;

        float now = Now();
        _startTime = now + Mathf.Max(0f, startDelay);

        if (cam && cam.orthographic && scaleOrthoSize)
            cam.orthographicSize = orthoStartSize;
    }

    void OnEnable()
    {
        float now = Now();
        _startTime = now + Mathf.Max(0f, startDelay);
        _currPitchDeg = startPitchDeg;
        _currYawOffset = yawBiasStartDeg;
        _yawVel = 0f;
        _pitchVel = 0f;
    }

    void LateUpdate()
    {
        if (!player) return;

        // ---- TIME → 0..1
        float now = Now();
        float elapsed = Mathf.Max(0f, now - _startTime);
        float dur = Mathf.Max(0.0001f, duration);
        float u = elapsed / dur; // unbounded

        float t;
        switch (cycleMode)
        {
            case CycleMode.Once:     t = Mathf.Clamp01(u); break;
            case CycleMode.Loop:     t = Mathf.Repeat(u, 1f); break;
            default: /* PingPong */  t = Mathf.PingPong(u, 1f); break;
        }

        // Optional easing for pitch & yaw bias
        float eased = pitchCurve != null ? pitchCurve.Evaluate(t) : t;

        // ---- PITCH (smoothed)
        float targetPitch = Mathf.Lerp(startPitchDeg, endPitchDeg, eased);
        _currPitchDeg = Mathf.SmoothDampAngle(
            _currPitchDeg,
            targetPitch,
            ref _pitchVel,
            1f / Mathf.Max(0.0001f, pitchSmooth)
        );

        // ---- YAW base (outward from tower OR fixed)
        float baseYawDeg = fixedYawDeg;
        if (towerCenter)
        {
            Vector3 radial = player.position - towerCenter.position;
            radial.y = 0f;
            if (radial.sqrMagnitude > 0.0001f)
            {
                baseYawDeg = Mathf.Atan2(radial.x, radial.z) * Mathf.Rad2Deg;
            }
        }

        // ---- YAW offset = bias (back→front) + sway (±)
        float bias = Mathf.Lerp(yawBiasStartDeg, yawBiasEndDeg, eased);
        float sway = yawSwayAmplitudeDeg * Mathf.Sin(elapsed * Mathf.PI * 2f * yawSwaySpeedHz);
        float targetYawOffset = bias + sway;

        float yawSmoothTime = (yawSmooth > 0f) ? 1f / yawSmooth : 0f;
        _currYawOffset = (yawSmoothTime > 0f)
            ? Mathf.SmoothDampAngle(_currYawOffset, targetYawOffset, ref _yawVel, yawSmoothTime)
            : targetYawOffset;

        float yawDeg = baseYawDeg + _currYawOffset;

        // ---- POSITION & ROTATION
        Quaternion rot = Quaternion.Euler(_currPitchDeg, yawDeg, 0f);
        Vector3 desiredPos = player.position - (rot * Vector3.forward * distance);

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionSmooth);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(player.position - transform.position, Vector3.up),
            Time.deltaTime * positionSmooth
        );

        // ---- ORTHO ZOOM (optional)
        if (cam && cam.orthographic && scaleOrthoSize)
        {
            float targetSize = Mathf.Lerp(orthoStartSize, orthoEndSize, eased);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * positionSmooth);
        }
    }

    float Now()
    {
        return useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    // Public utility if you want to restart the cycle from other scripts.
    public void Restart()
    {
        _startTime = Now() + Mathf.Max(0f, startDelay);
        _currPitchDeg = startPitchDeg;
        _currYawOffset = yawBiasStartDeg;
        _yawVel = 0f;
        _pitchVel = 0f;
    }
}
