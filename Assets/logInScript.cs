using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;
using TMPro;

public class logInScript : MonoBehaviour
{
    public GameObject iniciopanel; // Reference to the panel GameObject
    public GameObject inscripcionpanel;
    public TMP_InputField passInputField;
    public TMP_InputField userInputField;

    public TMP_InputField passSignUpInputField;
    public TMP_InputField userSignUpInputField;

    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        if (iniciopanel != null)
            iniciopanel.SetActive(false);

        if (inscripcionpanel != null)
            inscripcionpanel.SetActive(false);
    }



    // Method to be called by the button to show the panel
    public void ShowPanel1()
    {
        if (iniciopanel != null)
            iniciopanel.SetActive(true); // Make the panel visible
    }

    public void ShowPanel2()
    {
        if (iniciopanel != null)
            iniciopanel.SetActive(false);

        if (inscripcionpanel != null)
            inscripcionpanel.SetActive(true); // Make the panel visible
    }


    public async void SignIn()
    {
        await SignInWithUsernamePasswordAsync(userInputField.text, passInputField.text);
    }

    public async void SignUp()
    {
        await SignUpWithUsernamePasswordAsync(userSignUpInputField.text, passSignUpInputField.text);
    }


    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            Debug.Log("SignUp is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }


    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            Debug.Log("SignIn is successful.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

}
