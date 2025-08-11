using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject tower;
    public GameObject player;
    public GameObject rampsParent;
    
    public GameObject crystalPrefab;
    public float crystalOffsetY = 5.082f;
    public float crystalOfsetZ = 0.739f;

    public GameObject maxSpawnHeightTarget;

    public bool gameActive = false;
    public bool crystalActive = false;

    public int coins;

    public void StartGame()
    {
        gameActive = true;
        
        // fire player animation event
        player.GetComponent<Animator>().SetTrigger("GameStart");

        // start rotation
    }

    public void GenerateCrystal(Transform gameObjectTransform)
    {
        Vector3 basePosition = gameObjectTransform.position;
        Vector3 offset = new Vector3(0, crystalOffsetY, crystalOfsetZ);
        Instantiate(crystalPrefab, basePosition + offset, Quaternion.identity, rampsParent.transform);
        crystalActive = true;
    }
}
