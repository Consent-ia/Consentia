using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public static TabController Instance { get; private set; }

    [System.Serializable]
    public class TabPage
    {
        public string pageName; // Optional: for debugging
        public Image tabImage;
        public GameObject pageObject;
    }

    [SerializeField]
    private TabPage[] tabPages;

    [Header("Default Colors")]
    [SerializeField]
    private Color defaultActiveColor = Color.white;

    [SerializeField]
    private Color defaultInactiveColor = Color.gray;

    private TabPage currentActiveTab;

    [Header("Tab Click Sound")]
    [SerializeField]
    private AudioClip tabClickSound;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (tabPages.Length > 0)
        {
            ActivateTab(tabPages[0]);
        }
    }

    // PRIMARY METHOD - Activate tab by page GameObject
    public void ActivateTabByPage(GameObject pageObject)
    {
        TabPage targetTab = System.Array.Find(tabPages, tp => tp.pageObject == pageObject);
        
        if (targetTab != null)
        {
            ActivateTab(targetTab);
            PlayTabClickSound(); // Play sound when tab is activated
        }
        else
        {
            Debug.LogWarning($"Page '{pageObject.name}' not found in tab pages!");
        }
    }

    // Keep for UI Button compatibility (Inspector onClick events)
    public void ActivateTabByIndex(int index)
    {
        if (index >= 0 && index < tabPages.Length)
        {
            ActivateTab(tabPages[index]);
            PlayTabClickSound(); // Play sound when tab is activated
        }
        else
        {
            Debug.LogWarning($"Tab index {index} is out of range!");
        }
    }

    private void ActivateTab(TabPage targetTab)
    {
        // Deactivate all tabs
        foreach (var tabPage in tabPages)
        {
            tabPage.tabImage.color = defaultInactiveColor;
            tabPage.pageObject.SetActive(false);
        }

        // Activate target tab
        targetTab.tabImage.color = defaultActiveColor;
        targetTab.pageObject.SetActive(true);
        currentActiveTab = targetTab;
    }

    public void OnTabClicked(int tabIndex)
    {
        ActivateTabByIndex(tabIndex);
    }

    private void PlayTabClickSound()
    {
        // Simply call PlaySFX with the clip - SoundManager handles the rest
        if (SoundManager.Instance != null && tabClickSound != null)
        {
            SoundManager.Instance.PlaySFX(tabClickSound);
        }
    }

    // Helper: Get current active page
    public GameObject GetActivePage()
    {
        return currentActiveTab?.pageObject;
    }

    // Helper: Check if specific page is active
    public bool IsPageActive(GameObject pageObject)
    {
        return currentActiveTab?.pageObject == pageObject;
    }
}
