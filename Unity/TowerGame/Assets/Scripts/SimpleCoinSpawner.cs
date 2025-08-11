using UnityEngine;

public class SimpleCoinSpawner : MonoBehaviour
{
    public GameManager gameManager;
    [Header("Coin Settings")]
    public GameObject coinPrefab;
    [Range(0f, 1f)] public float spawnChance = 0.3f;

    [Header("Position Offsets")]
    public float radialOffset = 1.0f; // distance from pillar center on XZ
    public float lowHeightOffset = 1.0f; // ground-level coin
    public float highHeightOffset = 2.0f; // jump coin

    [Header("Scene Refs")]
    public Transform towerCenter; // null = world origin
    [Tooltip("Parent for spawned coins (set this to the same object your planks are parented to).")]
    public Transform coinParent;  // set to your ramp 'parent' transform

    [Header("High Coin Chance")]
    [Range(0f, 1f)] public float highCoinChance = 0.5f; // % of coins that spawn at jump height

    /// <summary>
    /// Call this when you spawn a ramp or gap.
    /// </summary>
    public void TrySpawnCoin(Vector3 segmentWorldPos, float segmentAngleDeg)
    {
        if (!coinPrefab) return;
        if (Random.value > spawnChance) return;

        // Pick height: low or high
        float heightOffset = (Random.value < highCoinChance) ? highHeightOffset : lowHeightOffset;

        Vector3 center = towerCenter ? towerCenter.position : Vector3.zero;

        // Outward direction based on the segment angle
        Quaternion segRot = Quaternion.Euler(0f, segmentAngleDeg, 0f);
        Vector3 outward = segRot * Vector3.forward;

        // Final spawn position
        Vector3 spawnPos = center + outward * radialOffset;
        spawnPos.y = segmentWorldPos.y + heightOffset;

        // Parent under the moving tower so coins flow with the track
        Transform parentForCoin = coinParent ? coinParent : transform;
        if (spawnPos.y < gameManager.maxSpawnHeightTarget.transform.position.y)
            Instantiate(coinPrefab, spawnPos, Quaternion.identity, parentForCoin);
    }
}