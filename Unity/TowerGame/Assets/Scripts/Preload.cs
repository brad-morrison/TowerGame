using UnityEngine;
using UnityEngine.SceneManagement;

public class Preload : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int pref = PlayerPrefs.GetInt("brand");
        if (pref == 0)
            PlayerPrefs.SetInt("brand", 1);
        
        SceneManager.LoadScene("brand");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
