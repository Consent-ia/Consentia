using System.Linq;
using UnityEngine;
using TMPro;

public class PrivacyProfileScreen : MonoBehaviour
{
    public static PrivacyProfileScreen Instance { get; private set; }

    [System.Serializable]
    public struct PrivacyCategory
    {
        public string categoryName;
        public TMP_Text iconLabel;
        public TMP_Text descriptionLabel;
        [TextArea(2, 4)] public string enabledText;
        [TextArea(2, 4)] public string disabledText;
    }


    [Header("Icon Characters")]
    [SerializeField] private string tickCharacter = "✓";
    [SerializeField] private string crossCharacter = "✗";

    [Header("Icon Colors")]
    [SerializeField] private Color tickColor = new Color(0.4f, 0.8f, 0.4f);
    [SerializeField] private Color crossColor = new Color(0.85f, 0.3f, 0.3f);

    [Header("Category Toggles (set by save data later)")]
    private const bool strictlyNecessary = true;
    private bool functionalPreferences = true;
    private bool analyticsPerformance = true;
    private bool marketingAdvertising = true;

    [Header("Categories")]
    [SerializeField] private PrivacyCategory strictlyNecessaryCategory = new PrivacyCategory
    {
        categoryName = "Strictly Necessary",
        enabledText  = "Always enabled. These services are essential for websites to function and cannot be disabled.",
        disabledText = "Always enabled. These services are essential for websites to function and cannot be disabled."
    };

    [SerializeField] private PrivacyCategory functionalPreferencesCategory = new PrivacyCategory
    {
        categoryName = "Functional / Preferences",
        enabledText  = "Enabled. Your choices indicated you are comfortable with services that remember your personal settings and preferences across visits.",
        disabledText = "Disabled. Your choices indicated you prefer not to use services that remember your personal settings and preferences across visits."
    };

    [SerializeField] private PrivacyCategory analyticsPerformanceCategory = new PrivacyCategory
    {
        categoryName = "Analytics / Performance",
        enabledText  = "Enabled. Your choices indicated you are comfortable sharing browsing behaviour data for site measurement purposes.",
        disabledText = "Disabled. Your choices indicated you prefer not to share browsing behaviour data for site measurement purposes."
    };

    [SerializeField] private PrivacyCategory marketingAdvertisingCategory = new PrivacyCategory
    {
        categoryName = "Marketing / Advertising",
        enabledText  = "Enabled. Your choices indicated you are comfortable with cross-site tracking for commercial targeting purposes.",
        disabledText = "Disabled. Your choices indicated you are not comfortable with cross-site tracking for commercial targeting purposes."
    };

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Hide();
    }

    public void Show()
    {
        SetupFromSaveData();
        Refresh();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible() => gameObject.activeSelf;

    public void Refresh()
    {
        ApplyCategory(strictlyNecessaryCategory,     strictlyNecessary);
        ApplyCategory(functionalPreferencesCategory, functionalPreferences);
        ApplyCategory(analyticsPerformanceCategory,  analyticsPerformance);
        ApplyCategory(marketingAdvertisingCategory,  marketingAdvertising);
    }

    private void ApplyCategory(PrivacyCategory category, bool value)
    {
        if (category.iconLabel)
        {
            category.iconLabel.text  = value ? tickCharacter : crossCharacter;
            category.iconLabel.color = value ? tickColor     : crossColor;
        }

        if (category.descriptionLabel)
            category.descriptionLabel.text = $"{category.categoryName}: {(value ? category.enabledText : category.disabledText)}";
    }
    
    private void SetupFromSaveData()
    {
        if (!SaveSystem.Instance) return;

        var result = SaveSystem.Instance.GetLastChoiceForAllNPCs();

        foreach (var npcName in from pair in result let npcName = pair.Key let choice = pair.Value where choice.choiceIndex == 1 select npcName)
        {
            functionalPreferences = npcName switch
            {
                "Lexi" or "Nox" or "Carta" or "Ada" or "Curio" => false,
                _ => functionalPreferences
            };
            analyticsPerformance = npcName switch
            {
                "Pulse" or "Aster" or "Brink" or "Quill" => false,
                _ => analyticsPerformance
            };
            marketingAdvertising = npcName switch
            {
                "Ledger" or "Mirr" or "Trace" or "Velor" => false,
                _ => marketingAdvertising
            };
        }
    }
}
