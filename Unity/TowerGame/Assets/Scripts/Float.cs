using UnityEngine;

public class FloatingLogo : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatAmplitude = 0.5f;  // How far it moves up and down
    public float floatFrequency = 1f;    // Speed of floating
    public float rotationAmplitude = 2f; // Small rotation tilt in degrees
    public float rotationFrequency = 1f; // Speed of rotation tilt

    private Vector3 startPos;
    private Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Update()
    {
        // Floating motion
        float yOffset = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;

        // Gentle rotation
        float rotZ = Mathf.Sin(Time.time * rotationFrequency) * rotationAmplitude;

        // Apply movement & rotation
        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
        transform.localRotation = startRot * Quaternion.Euler(0, 0, rotZ);
    }
}