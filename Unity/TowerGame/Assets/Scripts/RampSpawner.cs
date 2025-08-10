using System.Collections.Generic;
using UnityEngine;

public class RampSpawner : MonoBehaviour
{
    public GameObject parent;

    [Header("Ramp Settings")]
    public GameObject RampSegmentPrefab;
    public float segmentHeight = 0.5f;
    public float anglePerSegment = 10f;
    public float playerYOffset = 0.55f;

    [Header("Spiral")]
    public Transform spiralCenter;          // Optional: if null, uses this.transform
    public float radius = 2f;               // How wide the spiral is (tower diameter ~ 2*radius)
    public bool faceAlongTangent = true;    // Should the planks point along the spiral

    [Header("Build Rules")]
    public int numberOfChunks = 30;
    public int minRunLength = 5;
    public int maxRunLength = 12;

    [Range(0f, 1f)]
    public float gapFrequency = 0.4f;

    [Header("Jump Gaps")]
    public JumpGap smallJump = new JumpGap { type = JumpType.Small, sizeInSegments = 2 };
    public JumpGap mediumJump = new JumpGap { type = JumpType.Medium, sizeInSegments = 4 };
    public JumpGap largeJump = new JumpGap { type = JumpType.Large, sizeInSegments = 6 };

    [Range(0f, 1f)] public float mediumJumpChance = 0.3f;
    [Range(0f, 1f)] public float largeJumpChance = 0.1f;

    [Header("Dynamic Spawning")]
    public Transform player;
    public float spawnBuffer = 15f;    // How far ahead of player to generate
    public float despawnBuffer = 10f;  // How far below player to destroy

    [Header("Gameplay Balancing")]
    public int initialSafeChunks = 5;  // Number of chunks without any jumps

    // --- Coins (assign a SimpleCoinSpawner in the Inspector) ---
    // public SimpleCoinSpawner coinSpawner;

    private float currentHeight = 0f;
    private float currentAngle = 0f;

    private Queue<GameObject> activePlanks = new Queue<GameObject>();
    private int totalChunksSpawned = 0;

    void Start()
    {
        GenerateInitialRamp();
    }

    void Update()
    {
        // Keep spawning ramps ahead of the player
        while (currentHeight < player.position.y + spawnBuffer)
        {
            GenerateRampChunk();
        }

        // Despawn ramps that fall below the player
        while (activePlanks.Count > 0 && activePlanks.Peek().transform.position.y < player.position.y - despawnBuffer)
        {
            Destroy(activePlanks.Dequeue());
        }
    }

    public void GenerateInitialRamp()
    {
        currentHeight = player.position.y - playerYOffset; // Align top of first plank to player feet
        currentAngle = 0f;
        totalChunksSpawned = 0;

        for (int i = 0; i < numberOfChunks; i++)
        {
            GenerateRampChunk();
        }
    }

    private void GenerateRampChunk()
    {
        int runLength = Random.Range(minRunLength, maxRunLength + 1);
        var center = spiralCenter ? spiralCenter.position : transform.position;

        for (int j = 0; j < runLength; j++)
        {
            // Position on the spiral at the current angle & height
            Vector3 radial = Quaternion.Euler(0f, currentAngle, 0f) * (Vector3.forward * radius);
            Vector3 pos = new Vector3(center.x + radial.x, currentHeight, center.z + radial.z);

            // Orientation: tangent to the spiral or facing outward from the center
            Quaternion rot = faceAlongTangent
                ? Quaternion.Euler(0f, currentAngle + 90f, 0f) // +90 so "forward" points along the orbit
                : Quaternion.LookRotation(radial.normalized, Vector3.up);

            GameObject plank = Instantiate(RampSegmentPrefab, pos, rot, parent.transform);
            plank.name = $"Plank_{activePlanks.Count}";
            activePlanks.Enqueue(plank);

            // --- Coin over this plank (chance handled by SimpleCoinSpawner) ---
            // if (coinSpawner)
            // {
            //     // Drop coin at the plank position on the spiral:
            //     coinSpawner.TrySpawnCoin(pos, currentAngle);
            // }

            currentHeight += segmentHeight;
            currentAngle  += anglePerSegment;
        }

        // Add jump gap only if past safe chunks
        if (totalChunksSpawned >= initialSafeChunks && runLength >= 6 && Random.value < gapFrequency)
        {
            // Gap center before advancing
            float gapStartH = currentHeight;
            float gapStartA = currentAngle;

            int gapSize = GetGapSize();

            float centerH = gapStartH + segmentHeight * gapSize * 0.5f;
            float centerA = gapStartA + anglePerSegment * gapSize * 0.5f;

            // --- Coin over the gap center (optional chance handled by spawner) ---
            // if (coinSpawner)
            // {
            //     // Compute the world position for the gap center on the spiral:
            //     Vector3 gapRadial = Quaternion.Euler(0f, centerA, 0f) * (Vector3.forward * radius);
            //     Vector3 centerPos = new Vector3(center.x + gapRadial.x, centerH, center.z + gapRadial.z);
            //     coinSpawner.TrySpawnCoin(centerPos, centerA);
            // }

            // Advance past the actual gap
            currentHeight += segmentHeight * gapSize;
            currentAngle  += anglePerSegment * gapSize;
        }

        totalChunksSpawned++;
    }

    private int GetGapSize()
    {
        float roll = Random.value;

        if (roll < largeJumpChance)
            return largeJump.sizeInSegments;
        else if (roll < mediumJumpChance + largeJumpChance)
            return mediumJump.sizeInSegments;
        else
            return smallJump.sizeInSegments;
    }

    private void ClearChildren()
    {
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(child.gameObject);
            else
                Destroy(child.gameObject);
#else
            Destroy(child.gameObject);
#endif
        }

        activePlanks.Clear();
    }

    // ---- Editor Gizmos: preview the spiral path ----
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        var center = spiralCenter ? spiralCenter.position : transform.position;

        // Start height preview near the player's feet if available, otherwise from this transform
        float tempHeight = (player ? player.position.y - 0.55f : transform.position.y);
        float tempAngle  = 0f;

        // Draw predicted positions for a full "generate initial ramp" worth of segments
        int segmentsToPreview = numberOfChunks * Mathf.Max(1, (minRunLength + maxRunLength) / 2);

        for (int i = 0; i < segmentsToPreview; i++)
        {
            Vector3 radial = Quaternion.Euler(0f, tempAngle, 0f) * (Vector3.forward * radius);
            Vector3 pos = new Vector3(center.x + radial.x, tempHeight, center.z + radial.z);

            Gizmos.DrawSphere(pos, 0.08f);

            tempHeight += segmentHeight;
            tempAngle  += anglePerSegment;
        }
    }
}

public enum JumpType
{
    Small,
    Medium,
    Large
}

[System.Serializable]
public struct JumpGap
{
    public JumpType type;
    public int sizeInSegments;
}
