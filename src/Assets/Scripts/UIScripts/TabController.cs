using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
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

    void Start()
    {
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
