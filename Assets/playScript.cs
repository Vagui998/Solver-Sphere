using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class playScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Initialize Unity Services here, if not already done elsewhere
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Public method to change scene, to be called by a button
    public void ChangeToOverworld()
    {
        // Check if the user is signed in
        if (AuthenticationService.Instance.IsSignedIn)
        {
            // Clear PlayerPrefs if needed (uncomment next line if you decide to use it)
            // PlayerPrefs.DeleteAll();

            // Unload unused assets to clean up memory
            Resources.UnloadUnusedAssets();

            // Optional: Clear system garbage
            System.GC.Collect();

            // Load the Main_Overworld scene, you can use LoadSceneAsync for better performance in some cases
            SceneManager.LoadScene("Main_Overworld");
        }
        else
        {
            Debug.Log("User is not signed in.");  // Log a message if the user is not signed in
        }
    }
}
