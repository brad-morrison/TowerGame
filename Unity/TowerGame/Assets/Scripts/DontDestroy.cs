// File: DontDestroyMe.cs
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class DontDestroyMe : MonoBehaviour
{
    private const string ContainerName = "~DDOL";

    private static GameObject container;

    private void Awake()
    {
        if (transform.parent != null)
        {
            transform.SetParent(null, true);
        }

        if (container == null)
        {
            container = GameObject.Find(ContainerName);
            if (container == null)
            {
                container = new GameObject(ContainerName);
                DontDestroyOnLoad(container);
            }
        }

        transform.SetParent(container.transform, true);

        foreach (Transform child in container.transform)
        {
            if (child != transform && child.name == name)
            {
                Destroy(gameObject);
                return;
            }
        }

    }
}