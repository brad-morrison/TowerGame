using UnityEngine;

public class ButtonBrandColour : MonoBehaviour
{
    private int brand;
    public GameObject[] elements;

    public Material sw;
    public Material bos;
    public Material lloyds;
    private Material activeBrandColour;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        brand = PlayerPrefs.GetInt("brand");
        switch (brand)
        {
            case 1:
                activeBrandColour = bos;
                break;
            case 2:
                activeBrandColour = lloyds;
                break;
            case 3:
                activeBrandColour = sw;
                break;
            default:
                activeBrandColour = bos;
                break;
        }
        
        SetAll();
    }

    public void SetAll()
    {
        foreach (GameObject e in elements)
        {
            e.GetComponent<Renderer>().material = activeBrandColour;
        }
    }
}
