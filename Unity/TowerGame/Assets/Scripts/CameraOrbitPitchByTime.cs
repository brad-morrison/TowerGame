using UnityEngine;

public class CameraOrbitPitchByTime : MonoBehaviour
{
    public GameManager gameManager;
    public Transform player;
    public Transform towerCenter;

    public float distance = 12f;
    public float fixedYawDeg = 0f;
    public float positionSmooth = 6f;

    public float startPitchDeg = 25f;
    public float endPitchDeg = -8f;

    public enum CycleMode { Once, Loop, PingPong }
    public CycleMode cycleMode = CycleMode.Loop;

    public float duration = 10f;

    public float startDelay = 0f;

    public bool useUnscaledTime = false;

    public float pitchSmooth = 8f;

    public AnimationCurve pitchCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public float yawBiasStartDeg = -30f;
    public float yawBiasEndDeg   =  30f;
    public float yawSwayAmplitudeDeg = 20f;
    public float yawSwaySpeedHz = 0.25f;
    public float yawSmooth = 6f;

    public Camera cam;
    public bool scaleOrthoSize = false;
    public float orthoStartSize = 6f;
    public float orthoEndSize = 8.5f;

    float _pitchVel;
    float _currPitchDeg;

    float _yawVel;
    float _currYawOffset;

    float _startTime;

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

        if (!gameManager.gameActive) return;

        float now = Now();
        float elapsed = Mathf.Max(0f, now - _startTime);
        float dur = Mathf.Max(0.0001f, duration);
        float u = elapsed / dur;

        float t;
        switch (cycleMode)
        {
            case CycleMode.Once:     t = Mathf.Clamp01(u); break;
            case CycleMode.Loop:     t = Mathf.Repeat(u, 1f); break;
            default: t = Mathf.PingPong(u, 1f); break;
        }

         float eased = pitchCurve != null ? pitchCurve.Evaluate(t) : t;

        float targetPitch = Mathf.Lerp(startPitchDeg, endPitchDeg, eased);
        _currPitchDeg = Mathf.SmoothDampAngle(
            _currPitchDeg,
            targetPitch,
            ref _pitchVel,
            1f / Mathf.Max(0.0001f, pitchSmooth)
        );

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

        float bias = Mathf.Lerp(yawBiasStartDeg, yawBiasEndDeg, eased);
        float sway = yawSwayAmplitudeDeg * Mathf.Sin(elapsed * Mathf.PI * 2f * yawSwaySpeedHz);
        float targetYawOffset = bias + sway;

        float yawSmoothTime = (yawSmooth > 0f) ? 1f / yawSmooth : 0f;
        _currYawOffset = (yawSmoothTime > 0f)
            ? Mathf.SmoothDampAngle(_currYawOffset, targetYawOffset, ref _yawVel, yawSmoothTime)
            : targetYawOffset;

        float yawDeg = baseYawDeg + _currYawOffset;

        Quaternion rot = Quaternion.Euler(_currPitchDeg, yawDeg, 0f);
        Vector3 desiredPos = player.position - (rot * Vector3.forward * distance);

        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * positionSmooth);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.LookRotation(player.position - transform.position, Vector3.up),
            Time.deltaTime * positionSmooth
        );

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

    public void Restart()
    {
        _startTime = Now() + Mathf.Max(0f, startDelay);
        _currPitchDeg = startPitchDeg;
        _currYawOffset = yawBiasStartDeg;
        _yawVel = 0f;
        _pitchVel = 0f;
    }
}
