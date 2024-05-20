using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ActivatePlayerAfterDelay(scene.name));
    }

    private IEnumerator ActivatePlayerAfterDelay(string sceneName)
    {
        yield return new WaitForSeconds(0.1f); 

        if (sceneName == "Main_OverWorld")
        {
            PlayerManager.Instance.gameObject.SetActive(true);
        }
        else if (sceneName == "MiniJuego2" || sceneName == "MiniJuego3")
        {
            PlayerManager.Instance.gameObject.SetActive(false);
        }
        else
        {
            PlayerManager.Instance.gameObject.SetActive(true); 
        }
    }

    public void LoadFixedScene()
    {
        SceneManager.LoadScene("MiniJuego1");
    }

    public void LoadFixedScene2()
    {
        SceneManager.LoadScene("MiniJuego2");
    }

    public void LoadFixedScene3()
    {
        SceneManager.LoadScene("MiniJuego3");
    }

    public void LoadFixedSceneMain()
    {
        SceneManager.LoadScene("Main_OverWorld");
    }
}
