using System.IO;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }
    
    private string saveFilePath;
    private PlayerChoicesSaveData saveData;
    private string currentSlotId;
    
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Load last used slot (optional). If none exists, we'll fall back to a default single-file save.
        currentSlotId = PlayerPrefs.GetString("CurrentSlotId", "");
        UpdateSaveFilePath();

        // Load existing data or create new
        LoadData();
    }

    /// <summary>
    /// Sets the active save slot for dialog choices.
    /// Call this when starting a New Game slot or before loading a slot.
    /// </summary>
    public void SetCurrentSlot(string slotId)
    {
        currentSlotId = slotId;
        PlayerPrefs.SetString("CurrentSlotId", currentSlotId);
        PlayerPrefs.Save();

        UpdateSaveFilePath();
        LoadData();
    }

    public string CreateNewSlotId()
    {
        return System.DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
    }

    private void UpdateSaveFilePath()
    {
        // Backwards compatibility: if no slot is chosen, use the old single file.
        saveFilePath = string.IsNullOrEmpty(currentSlotId)
            ? Path.Combine(Application.persistentDataPath, "playerChoices.json")
            : Path.Combine(Application.persistentDataPath, $"playerChoices_{currentSlotId}.json");
    }
    
    public void SaveChoice(string npcName, string questionText, string selectedChoice, int choiceIndex)
    {
        DialogChoiceData choiceData = new DialogChoiceData(npcName, questionText, selectedChoice, choiceIndex);
        saveData.choices.Add(choiceData);
        
        SaveData();
        
        Debug.Log($"Saved choice: {selectedChoice} for NPC: {npcName}");
    }
    
    private void SaveData()
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Data saved to: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }
    
    private void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            try
            {
                string json = File.ReadAllText(saveFilePath);
                saveData = JsonUtility.FromJson<PlayerChoicesSaveData>(json);
                Debug.Log($"Data loaded from: {saveFilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to load data: {e.Message}");
                saveData = new PlayerChoicesSaveData();
            }
        }
        else
        {
            saveData = new PlayerChoicesSaveData();
            Debug.Log("No save file found. Creating new save data.");
        }
    }
    
    public PlayerChoicesSaveData GetSaveData()
    {
        return saveData;
    }
    
    public DialogChoiceData GetLastChoiceForNPC(string npcName)
    {
        for (int i = saveData.choices.Count - 1; i >= 0; i--)
        {
            if (saveData.choices[i].npcName == npcName)
            {
                return saveData.choices[i];
            }
        }
        return null;
    }
    
    public bool HasMadeChoice(string npcName, string questionText)
    {
        return saveData.choices.Any(choice => choice.npcName == npcName && choice.questionText == questionText);
    }
    
    public void ClearAllChoices()
    {
        saveData.choices.Clear();
        SaveData();
        Debug.Log("All choices cleared.");
    }
    
    public void DeleteSaveFile()
    {
        if (!File.Exists(saveFilePath)) 
            return;
        File.Delete(saveFilePath);
        saveData = new PlayerChoicesSaveData();
        Debug.Log("Save file deleted.");
    }
}