using System.Collections.Generic;
using UnityEngine;

public class PillarSpawner : MonoBehaviour
{
    public GameObject pillarPrefab;
    public GameObject pillarParent;
    public float segmentHeight = 0.5f;
    public Transform player;
    public float spawnBuffer = 15f;
    public float despawnBuffer = 10f;

    private float currentHeight = 0f;
    private Queue<GameObject> activePillars = new Queue<GameObject>();

    void Update()
    {
        // Spawn pillar segments until we reach player + buffer
        while (currentHeight < player.position.y + spawnBuffer)
        {
            Vector3 pos = new Vector3(0, currentHeight, 0);
            GameObject pillar = Instantiate(pillarPrefab, pos, Quaternion.identity, transform);
            pillar.name = $"Pillar_{activePillars.Count}";
            pillar.transform.parent = pillarParent.transform;

            activePillars.Enqueue(pillar);
            currentHeight += segmentHeight;
        }

        // Despawn pillars that are below the player
        while (activePillars.Count > 0 && activePillars.Peek().transform.position.y < player.position.y - despawnBuffer)
        {
            Destroy(activePillars.Dequeue());
        }
    }
}