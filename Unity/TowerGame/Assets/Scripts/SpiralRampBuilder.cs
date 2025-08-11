using System.Collections.Generic;
using UnityEngine;

public class SpiralRampBuilder : MonoBehaviour
{
    public GameObject parent;
    public GameManager gameManager;
    public GameObject crystalPrefab;

    [Header("Ramp Settings")]
    public GameObject plankPrefab;
    public float segmentHeight = 0.5f;
    public float anglePerSegment = 10f;

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
    public SimpleCoinSpawner coinSpawner;

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
        currentHeight = player.position.y - 0.55f; // Align top of first plank to player feet
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

        for (int j = 0; j < runLength; j++)
        {
            
            Vector3 pos = new Vector3(0, currentHeight, 0);
            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);

            if (currentHeight < gameManager.maxSpawnHeightTarget.transform.position.y)
            {
                GameObject plank = Instantiate(plankPrefab, pos, rot, parent.transform);
                plank.name = $"Plank_{activePlanks.Count}";
                activePlanks.Enqueue(plank);
            }
            else
            {
                if (!gameManager.crystalActive)
                    gameManager.GenerateCrystal(parent.transform.GetChild(parent.transform.childCount - 5));
            }
            

            // --- Coin over this plank (chance handled by SimpleCoinSpawner) ---
            if (coinSpawner)
            {
                coinSpawner.TrySpawnCoin(pos, currentAngle);
            }

            currentHeight += segmentHeight;
            currentAngle += anglePerSegment;
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
            if (coinSpawner)
            {
                Vector3 centerPos = new Vector3(0f, centerH, 0f);
                coinSpawner.TrySpawnCoin(centerPos, centerA);
            }

            // Advance past the actual gap
            currentHeight += segmentHeight * gapSize;
            currentAngle += anglePerSegment * gapSize;
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
