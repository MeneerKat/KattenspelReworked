using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // Switch scene when called
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}