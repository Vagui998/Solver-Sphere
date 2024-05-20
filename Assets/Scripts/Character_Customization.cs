using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class Character_Customization : MonoBehaviour
{
    public Player player;
    public static Character_Customization instance;

    [SerializeField] private SkinnedMeshRenderer beardRenderer;
    [SerializeField] private SkinnedMeshRenderer skinRenderer;
    [SerializeField] private SkinnedMeshRenderer hairRenderer;

    [SerializeField] private SkinnedMeshRenderer leftBootRenderer;
    [SerializeField] private SkinnedMeshRenderer rightBootRenderer;
    [SerializeField] private SkinnedMeshRenderer leftPantRenderer;
    [SerializeField] private SkinnedMeshRenderer rightPantRenderer;

    [SerializeField] private SkinnedMeshRenderer topRenderer;
    [SerializeField] private SkinnedMeshRenderer leftShoulderRenderer;
    [SerializeField] private SkinnedMeshRenderer rightShoulderRenderer;

    private Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"Rojo", "#E74C3C"},
        {"Naranja", "#E78A3C"},
        {"Amarillo", "#F1C40F"},
        {"Azul", "#07396B"},
        {"Morado", "#7A3CE7"},
        {"Negro", "#1A1A1A"},
        {"Blanco", "#FFFFFF"},
    };

    private Dictionary<string, string> skinColors = new Dictionary<string, string>()
    {
        {"Piel 1", "#c58c85"},
        {"Piel 2", "#ecbc4"},
        {"Piel 3", "#d1a3a4"},
        {"Piel 4", "#a1665e"},
        {"Piel 5", "#503335"},
        {"Piel 6", "#592f2a"},
    };

    private int hairID;
    private int skinID;
    private int beardID;
    private int topID;
    private int pantsID;
    private int bootsID;

    [SerializeField] private TextMeshProUGUI hairText;
    [SerializeField] private TextMeshProUGUI skinText;
    [SerializeField] private TextMeshProUGUI beardText;
    [SerializeField] private TextMeshProUGUI topText;
    [SerializeField] private TextMeshProUGUI pantsText;
    [SerializeField] private TextMeshProUGUI bootsText;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        if (player != null)
        {
            LoadCustomization();
        }
        else
        {
            Debug.LogWarning("No se encontró el objeto del jugador en la escena.");
        }
    }

    private async void LoadCustomization()
    {
        if (!this) return;  // Check if the script is still active
        try
        {
            PersistenceManager.CustomizationData data = await PersistenceManager.LoadCustomizationData();

            if (data != null && this)  // Check again in case the object was destroyed during the await
            {
                ApplyCustomizationData(data);
            }
            else
            {
                Debug.LogWarning("Customization data is null.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load customization data: " + e.Message);
        }
    }

    private void ApplyCustomizationData(PersistenceManager.CustomizationData data)
    {
        if (!hairRenderer || !skinRenderer || !beardRenderer) return;  // Check if any renderers are null

        // Apply the loaded customization data
        UpdateMaterialColor(hairRenderer, data.hairID, hairText);
        UpdateSkinColor(skinRenderer, data.skinID, skinText);
        UpdateMaterialColor(beardRenderer, data.beardID, beardText);
        UpdateUpperMaterialColor(leftShoulderRenderer, topRenderer, rightShoulderRenderer, data.topID, topText);
        UpdateLowerMaterialColor(leftPantRenderer, rightPantRenderer, data.pantsID, pantsText);
        UpdateLowerMaterialColor(leftBootRenderer, rightBootRenderer, data.bootsID, bootsText);
    }

    private void SaveCustomization()
    {
        PersistenceManager.CustomizationData data = new PersistenceManager.CustomizationData(hairID, skinID, beardID, topID, pantsID, bootsID);
        PersistenceManager.SaveCustomizationData(data);
        Debug.Log("Personalización guardada correctamente.");
    }

    public void SelectHair(bool isForward)
    {
        ChangeColor("hair", isForward);
        SaveCustomization(); // Save after change
    }

    public void SelectSkin(bool isForward)
    {
        ChangeSkinColor(isForward);
        SaveCustomization(); // Save after change
    }

    public void SelectBeard(bool isForward)
    {
        ChangeColor("beard", isForward);
        SaveCustomization();
    }
    public void SelectTop(bool isForward)
    {
        ChangeColor("top", isForward);
        SaveCustomization();
    }

    public void SelectPants(bool isForward)
    {
        ChangeColor("pants", isForward);
        SaveCustomization();
    }

    public void SelectBoots(bool isForward)
    {
        ChangeColor("boots", isForward);
        SaveCustomization();
    }

    public void ChangeSkinColor(bool isForward)
    {
        skinID = UpdateSkinId(skinID, isForward);
        UpdateSkinColor(skinRenderer, skinID, skinText);
    }

    public void ChangeColor(string type, bool isForward)
    {
        switch (type)
        {
            case "hair":
                hairID = UpdateID(hairID, isForward);
                UpdateMaterialColor(hairRenderer, hairID, hairText);
                break;

            case "beard":
                beardID = UpdateID(beardID, isForward);
                UpdateMaterialColor(beardRenderer, beardID, beardText);
                break;

            case "top":
                topID = UpdateID(topID, isForward);
                UpdateUpperMaterialColor(leftShoulderRenderer, topRenderer, rightShoulderRenderer , topID, topText);
                break;

            case "pants":
                pantsID = UpdateID(pantsID, isForward);
                UpdateLowerMaterialColor(leftPantRenderer, rightPantRenderer, pantsID, pantsText);
                break;

            case "boots":
                bootsID = UpdateID(bootsID, isForward);
                UpdateLowerMaterialColor(leftBootRenderer, rightBootRenderer, bootsID, bootsText);
                break;


        }
    }

    private int UpdateSkinId( int id, bool isForward)
    {
        if (isForward)
        {
            id = (id + 1) % skinColors.Count;
        }
        else
        {
            id = (id - 1 + skinColors.Count) % skinColors.Count;
        }
        return id;
    }

    private int UpdateID(int id, bool isForward)
    {
        if (isForward)
        {
            id = (id + 1) % colors.Count;
        }
        else
        {
            id = (id - 1 + colors.Count) % colors.Count;
        }
        return id;
    }

    public void UpdateMaterialColor(SkinnedMeshRenderer renderer, int id, TextMeshProUGUI text)
    {
        string colorName = colors.Keys.ElementAt(id);
        text.text = colorName;
        if (ColorUtility.TryParseHtmlString(colors.Values.ElementAt(id), out Color color))
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                material.SetColor("_Color", color); // Cambiar a "_BaseColor" si se utiliza el shader URP/Lit
            }
            renderer.materials = materials;
        }
    }

    public void UpdateSkinColor( SkinnedMeshRenderer renderer, int id, TextMeshProUGUI text)
    {
        string colorname = skinColors.Keys.ElementAt(id);
        text.text = colorname;
        if (ColorUtility.TryParseHtmlString(skinColors.Values.ElementAt(id), out Color color))
        {
            Material[] materials = renderer.materials;
            foreach (Material material in materials)
            {
                material.SetColor("_Color", color); // Cambiar a "_BaseColor" si se utiliza el shader URP/Lit
            }
            renderer.materials = materials;
        }
    }

    public void UpdateLowerMaterialColor(SkinnedMeshRenderer leftRenderer, SkinnedMeshRenderer rightRenderer, int id, TextMeshProUGUI txt)
    {
        string colorName = colors.Keys.ElementAt(id);
        txt.text = colorName;

        if (ColorUtility.TryParseHtmlString(colors.Values.ElementAt(id), out Color color))
        {
            Material[] leftMaterials = leftRenderer.materials;
            Material[] rightMaterials = rightRenderer.materials;

            foreach (Material material in leftMaterials)
            {
                material.SetColor("_Color", color);
            }

            foreach (Material material in rightMaterials)
            {
                material.SetColor("_Color", color);
            }

            leftRenderer.materials = leftMaterials;
            rightRenderer.materials = rightMaterials;
        }
    }

    public void UpdateUpperMaterialColor(SkinnedMeshRenderer leftRenderer, SkinnedMeshRenderer midRenderer,SkinnedMeshRenderer rightRenderer, int id, TextMeshProUGUI txt)
    {
        string colorName = colors.Keys.ElementAt(id);
        txt.text = colorName;

        if(ColorUtility.TryParseHtmlString(colors.Values.ElementAt(id), out Color color))
        {
            Material[] leftMaterials = leftRenderer.materials;
            Material[] rightMaterials = rightRenderer.materials;
            Material[] midMaterials = midRenderer.materials;

            foreach (Material material in leftMaterials)
            {
                material.SetColor("_Color", color);
            }

            foreach (Material material in rightMaterials)
            {
                material.SetColor("_Color", color);
            }

            foreach(Material material in midMaterials)
            {
                material.SetColor("_Color", color);
            }

            leftRenderer.materials = leftMaterials;
            rightRenderer.materials = rightMaterials;
            midRenderer.materials = midMaterials;
        }
    }

}
