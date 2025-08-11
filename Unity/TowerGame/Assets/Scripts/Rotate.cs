using UnityEngine;

public class OrbitAroundTarget : MonoBehaviour
{
    [Tooltip("The object to orbit around (its origin/pivot).")]
    public Transform target;

    [Tooltip("Degrees per second. Negative = opposite direction.")]
    public float speed = 45f;

    [Tooltip("Axis to orbit around. (0,1,0) = around Y/up")]
    public Vector3 axis = Vector3.up;

    public enum AxisSpace { World, TargetLocal }

    [Tooltip("Interpret the axis in world space or relative to the target's local axes.")]
    public AxisSpace axisSpace = AxisSpace.World;

    void OnValidate()
    {
        if (axis.sqrMagnitude < 1e-6f) axis = Vector3.up;
        else axis = axis.normalized;
    }

    void Update()
    {
        if (!target) return;

        // Use world axis as-is, or convert from target's local space to world.
        Vector3 worldAxis = axisSpace == AxisSpace.World ? axis : target.TransformDirection(axis);

        transform.RotateAround(target.position, worldAxis, speed * Time.deltaTime);
    }
}