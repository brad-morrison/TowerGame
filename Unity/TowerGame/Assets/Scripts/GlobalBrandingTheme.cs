using System;
using UnityEngine;

public class GlobalBrandingTheme : MonoBehaviour
{
    public int activeBrand = 1;
    public GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        InitialBrandSetting();
    }

    public void SetBrandChoice(int brand)
    {
        activeBrand = brand;
        gameManager.activeBrand = brand;
        PlayerPrefs.SetInt("brand", brand);
    }

    public void InitialBrandSetting()
    {
        int pref = PlayerPrefs.GetInt("brand");
        if (pref == 0)
            SetBrandChoice(1);
        else
            SetBrandChoice(pref);
        
        //SetBrandChoice(2);
    }
}
