using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    void Start()
    {
        int pref = PlayerPrefs.GetInt("brand");
        if (pref == 0)
            PlayerPrefs.SetInt("brand", 1);
        
        SceneManager.LoadScene("brand");
    }
}
