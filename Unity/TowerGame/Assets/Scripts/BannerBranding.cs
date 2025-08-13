using UnityEngine;

public class BannerBranding : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject flag_bos, flag_sw, flag_lloyds;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (gameManager.activeBrand)
        {
            case 1:
                flag_bos.SetActive(true);
                break;
            case 2:
                flag_lloyds.SetActive(true);
                break;
            case 3:
                flag_sw.SetActive(true);
                break;
            default:
                flag_bos.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
