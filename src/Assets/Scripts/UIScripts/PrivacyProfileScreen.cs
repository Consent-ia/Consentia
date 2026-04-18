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
    public bool strictlyNecessary = true;
    public bool functionalPreferences = true;
    public bool analyticsPerformance = false;
    public bool marketingAdvertising = false;

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
}
