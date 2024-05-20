using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Threading.Tasks;

public class PersistenceManager : MonoBehaviour
{
    void Awake()
    {
        LoadCharacterCustomization();
    }

    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.Log("You are not logged in.");
                // Optionally, try to sign in anonymously or simply return to avoid any further actions
                // await AuthenticationService.Instance.SignInAnonymouslyAsync();
                return; // Return early if not signed in
            }

            // If signed in, you can proceed with loading or saving data as necessary
            Debug.Log("User is logged in, proceeding with data operations.");
            // Call methods to load or save data here if needed
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error initializing Unity Services: " + e.Message);
        }
    }


    public static async Task SaveCustomizationData(CustomizationData data)
    {
        // Adjust the dictionary to store object values, even though we use strings
        Dictionary<string, object> dataDict = new Dictionary<string, object>
    {
        {"hairID", data.hairID.ToString()},
        {"skinID", data.skinID.ToString()},
        {"beardID", data.beardID.ToString()},
        {"topID", data.topID.ToString()},
        {"pantsID", data.pantsID.ToString()},
        {"bootsID", data.bootsID.ToString()}
    };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataDict);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save customization data: " + e.Message);
        }
    }


    public static async Task<CustomizationData> LoadCustomizationData()
    {
        try
        {
            var loadData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>
            {
                "hairID", "skinID", "beardID", "topID", "pantsID", "bootsID"
            });

            if (loadData.Count > 0)
            {
                return new CustomizationData(
                    int.Parse(loadData["hairID"]),
                    int.Parse(loadData["skinID"]),
                    int.Parse(loadData["beardID"]),
                    int.Parse(loadData["topID"]),
                    int.Parse(loadData["pantsID"]),
                    int.Parse(loadData["bootsID"])
                );
            }
            else
            {
                Debug.Log("No customization data found.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load customization data: " + e.Message);
        }
        return null;
    }

    void LoadCharacterCustomization()
    {
        LoadCustomizationData().ContinueWith(task =>
        {
            CustomizationData customizationData = task.Result;
            if (customizationData != null)
            {
                ApplyCharacterCustomization(customizationData);
            }
            else
            {
                Debug.LogWarning("No customization data found when loading the character.");
            }
        });
    }

    void ApplyCharacterCustomization(CustomizationData data)
    {
        Debug.Log("Customization applied to character: " + JsonUtility.ToJson(data));
    }

    [System.Serializable]
    public class CustomizationData
    {
        public int hairID;
        public int skinID;
        public int beardID;
        public int topID;
        public int pantsID;
        public int bootsID;

        public CustomizationData(int hairID, int skinID, int beardID, int topID, int pantsID, int bootsID)
        {
            this.hairID = hairID;
            this.skinID = skinID;
            this.beardID = beardID;
            this.topID = topID;
            this.pantsID = pantsID;
            this.bootsID = bootsID;
        }
    }
}
