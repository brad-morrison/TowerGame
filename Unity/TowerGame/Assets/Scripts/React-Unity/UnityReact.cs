using System;
using UnityEngine;

public class UnityReact : MonoBehaviour
{
    
    public void GameOverUI(bool show)
    {
        if (show)
        {
            
        }
        else
        {
            
        }
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName == "game")
        {
            
        }

        if (sceneName == "menu")
        {
            
        }
    }
    
    // call to React
    public void React_GameOverUI(bool show) {
        Debug.Log("React_GameOverUI" + show);
        if (show)
        {
            Application.ExternalCall("GameOver_Show");
        }
        else
        {
            Application.ExternalCall("GameOver_Hide");
        }
    }
}
