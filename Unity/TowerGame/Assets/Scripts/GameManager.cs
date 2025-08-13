using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public int activeBrand = 3;
    public Notification Notification;

    public GameObject tower;
    public GameObject player;
    public GameObject rampsParent;
    public CounterUI counterUI;
    public int coinCount;
    public int tokenCount;


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
    public GameObject HelpUI;

    public bool uiOpen = false;

    private void Awake()
    {
        Time.timeScale = 1f; // unpause the game
    }

    public void StartGame()
    {
        HelpUI.SetActive(false);
        uiOpen = false;
        /*
        volume.profile.TryGet<DepthOfField>(out dof);
        if (SceneManager.GetActiveScene().name == "home")   // exact case
            dof.focusDistance.value = 4.11f;
        else
            dof.focusDistance.value = 3.0f;
        */

        gameActive = true;

        // fire player animation event
        player.GetComponent<Animator>().SetTrigger("GameStart");

        // start rotation
    }

    public void GameWin()
    {
        uiOpen = true;
        gameActive = false;
        //StartCoroutine(PauseAfterDelay(0.1f));
        counterUI.SetWinUI();
        player.GetComponent<SimpleJump>().StopRunningSFX();
        WinUI.SetActive(true);
        //dof.focusDistance.value = 1.6f;
        
    }

    public void GameOver()
    {
        uiOpen = true;
        gameActive = false;
        StartCoroutine(PauseAfterDelay(0.1f));
        counterUI.SetGameOverUI();
        LoseUI.SetActive(true);
        //dof.focusDistance.value = 1.6f;
        HideCharacter();
    }

    public void GenerateCrystal(Transform gameObjectTransform)
    {
        Vector3 basePosition = gameObjectTransform.position;
        Vector3 offset = new Vector3(0, crystalOffsetY, crystalOfsetZ);
        Instantiate(crystalPrefab, basePosition + offset, Quaternion.identity, rampsParent.transform);
        crystalActive = true;
    }

    public void StopAllMovingElements()
    {
        gameActive = false;
    }

    public void HideCharacter()
    {
        player.SetActive(false);
    }

IEnumerator PauseAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        print("PAUSED");
        StopAllMovingElements();
        //react.React_GameOverUI(true, gameManager.coins);
    }
}
