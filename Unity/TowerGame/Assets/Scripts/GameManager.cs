using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    public Volume volume;
    public DepthOfField dof;

    public int coins;

    public GameObject WinUI, LoseUI;

    private void Awake()
    {
        Time.timeScale = 1f; // Pause the game
    }

    public void StartGame()
    {
        volume.profile.TryGet<DepthOfField>(out dof);
        if (SceneManager.GetActiveScene().name == "home")   // exact case
            dof.focusDistance.value = 4.11f;
        else
            dof.focusDistance.value = 3.0f;
            
        
        gameActive = true;
        
        // fire player animation event
        player.GetComponent<Animator>().SetTrigger("GameStart");

        // start rotation
    }

    public void GameWin()
    {
        gameActive = false;
        StartCoroutine(PauseAfterDelay(0.1f));
        player.GetComponent<SimpleJump>().StopRunningSFX();
        WinUI.SetActive(true);
        dof.focusDistance.value = 1.6f;
    }

    public void GameOver()
    {
        gameActive = false;
        LoseUI.SetActive(true);
        dof.focusDistance.value = 1.6f;
    }

    public void GenerateCrystal(Transform gameObjectTransform)
    {
        Vector3 basePosition = gameObjectTransform.position;
        Vector3 offset = new Vector3(0, crystalOffsetY, crystalOfsetZ);
        Instantiate(crystalPrefab, basePosition + offset, Quaternion.identity, rampsParent.transform);
        crystalActive = true;
    }
    
    IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        print("PAUSED");
        Time.timeScale = 0f; // Pause the game
        //react.React_GameOverUI(true, gameManager.coins);
    }
}
