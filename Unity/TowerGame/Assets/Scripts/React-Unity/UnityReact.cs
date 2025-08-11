using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

public class UnityReact : MonoBehaviour
{
    [Preserve]
    public void LoadScene(string sceneName)
    {
        Debug.Log("Calling LoadScene : " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
    
    // call to React
    public void React_GameOverUI(bool show, int score) {
        Debug.Log("React_GameOverUI" + show);
        if (show)
        {
            Application.ExternalCall("GameOver_Show", score);
        }
        else
        {
            Application.ExternalCall("GameOver_Hide");
        }
    }

    public void React_GameWonUI(bool show, int score)
    {
        Debug.Log("React_GameWonUI" + show);
        if (show)
        {
            Application.ExternalCall("GameWon_Show", score);
        }
        else
        {
            Application.ExternalCall("GameWon_Hide");
        }
    }
}
