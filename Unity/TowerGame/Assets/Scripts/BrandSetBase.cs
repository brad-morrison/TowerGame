using UnityEngine;
using UnityEngine.SceneManagement;

public class BrandSetBase : MonoBehaviour
{
    public void SetBrandChoice(int brand)
    {
        // set pref
        PlayerPrefs.SetInt("brand", brand);
        
        // load home
        SceneManager.LoadScene("home");
    }
}
