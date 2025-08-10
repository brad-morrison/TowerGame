using UnityEngine;

public class TriggerProbe : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"[Probe] Player hit {other.name}");
    }
}