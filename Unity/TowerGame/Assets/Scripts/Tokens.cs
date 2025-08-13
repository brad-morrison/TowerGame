using System;
using UnityEngine;

public class Tokens : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject[] tokens_bos;
    public GameObject[] tokens_sw;
    public GameObject[] tokens_lloyds;

    [Tooltip("UI Screens")] 
    public GameObject[] tokens_ui_bos;
    public GameObject[] tokens_ui_sw;
    public GameObject[] tokens_ui_lloyds;

    private void Start()
    {
        ActivateBrand();
    }

    public void ActivateBrand()
    {
        switch (gameManager.activeBrand)
        {
            case 1: ActivateBrand_bos();
                break;
            case 2: ActivateBrand_lloyds();
                break;
            case 3: ActivateBrand_sw();
                break;
            default:
                ActivateBrand_sw();
                break;
        }
    }

    public void ActivateBrand_bos()
    {
        foreach (GameObject token in tokens_bos)
        {
            if (token != null)
                token.SetActive(true);
        }
        
        foreach (GameObject token in tokens_ui_bos)
        {
            if (token != null)
                token.SetActive(true);
        }
    }
    
    public void ActivateBrand_lloyds()
    {
        foreach (GameObject token in tokens_lloyds)
        {
            if (token != null)
                token.SetActive(true);
        }
        
        foreach (GameObject token in tokens_ui_lloyds)
        {
            if (token != null)
                token.SetActive(true);
        }
    }
    
    public void ActivateBrand_sw()
    {
        foreach (GameObject token in tokens_sw)
        {
            if (token != null)
                token.SetActive(true);
        }
        
        foreach (GameObject token in tokens_ui_sw)
        {
            if (token != null)
                token.SetActive(true);
        }
    }
}