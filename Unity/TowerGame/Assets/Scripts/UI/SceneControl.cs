using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneControl : MonoBehaviour
{
    [SerializeField] bool loadAsync = false;  // toggle if you want async loads

    public void LoadSceneByName(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("[SceneControl] Scene name is empty.");
            return;
        }

        if (!SceneExists(sceneName))
        {
            Debug.LogError($"[SceneControl] Scene '{sceneName}' is not in Build Settings.");
            return;
        }

        if (loadAsync) StartCoroutine(LoadAsync(sceneName));
        else SceneManager.LoadScene(sceneName);
    }

    private bool SceneExists(string name)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        for (int i = 0; i < count; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var n = System.IO.Path.GetFileNameWithoutExtension(path);
            if (n == name) return true;
        }
        return false;
    }

    private IEnumerator LoadAsync(string name)
    {
        var op = SceneManager.LoadSceneAsync(name);
        while (!op.isDone) yield return null;
    }

    // Optional helper if you want a quit button
    public void QuitApp() => Application.Quit();
}