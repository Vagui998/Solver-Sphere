using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject customizationPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Vector3 playerPosition;
    [SerializeField] private Quaternion playerRotation;
    [SerializeField] private string sceneName;



    public void PausePanel()
    {
        Time.timeScale = 0;
        pauseButton.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ReturnGame()
    {
        Time.timeScale = 1;
        pauseButton.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void SettingsPanel()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void AcceptSettings()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void ChangingVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    public void ActivateCustomizationPanel()
    {
        customizationPanel.SetActive(true);
    }

    public void AcceptCustomizationPanel()
    {
        // Desactivar el panel de personalización
        customizationPanel.SetActive(false);

        // Cargar la nueva escena
        SceneManager.LoadScene("Interior_1");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(gameObject.tag);
        Debug.Log(other.tag);
        if (other.CompareTag("Player"))
        {
            // Cargar la escena del guardarropa si se ha configurado
            if (!string.IsNullOrEmpty(sceneName) && sceneName != "Main_OverWorld")
            {
                other.transform.position = playerPosition;
                other.transform.rotation = playerRotation;
                Resources.UnloadUnusedAssets();
                SceneManager.LoadScene(sceneName);
            }
            else if (!string.IsNullOrEmpty(sceneName) && sceneName == "Main_OverWorld" && gameObject.tag == "Red_Door")
            {
                other.transform.position = playerPosition;
                other.transform.rotation = playerRotation;
                Resources.UnloadUnusedAssets();
                SceneManager.LoadScene(sceneName);
            }
            else 
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}
