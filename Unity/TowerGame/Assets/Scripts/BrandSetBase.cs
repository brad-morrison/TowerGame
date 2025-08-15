using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BrandSetBase : MonoBehaviour
{
    public void SetBrandChoice(int brand)
    {
        PlayerPrefs.SetInt("brand", brand);
        
        SceneManager.LoadScene("home");
    }
}
