// File: DontDestroyMe.cs
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class DontDestroyMe : MonoBehaviour
{
    private const string ContainerName = "~DDOL";

    private static GameObject container;

    private void Awake()
    {
        // 1) Make sure we're a root object so DDOL works reliably
        if (transform.parent != null)
        {
            transform.SetParent(null, true); // keep world position
        }

        // 2) Ensure a single global container that is itself DDOL
        if (container == null)
        {
            container = GameObject.Find(ContainerName);
            if (container == null)
            {
                container = new GameObject(ContainerName);
                DontDestroyOnLoad(container); // this is the true DDOL root
                Debug.Log("[DontDestroyMe] Created DDOL container.");
            }
        }

        // 3) Reparent into the DDOL container
        transform.SetParent(container.transform, true);

        // 4) Optional: if another copy of THIS object exists, kill the duplicate
        foreach (Transform child in container.transform)
        {
            if (child != transform && child.name == name)
            {
                Debug.Log($"[DontDestroyMe] Duplicate '{name}' found. Destroying the newcomer.");
                Destroy(gameObject);
                return;
            }
        }

        Debug.Log($"[DontDestroyMe] '{name}' persisted in scene '{gameObject.scene.name}'.");
    }
}