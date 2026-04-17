using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private string saveFilePath;
    private PlayerChoicesSaveData saveData;

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

        // Single-slot overwrite save.
        saveFilePath = Path.Combine(Application.persistentDataPath, "playerChoices.json");

        // Load existing data or create new
        LoadData();
    }

    // (Slots removed) If you need multiple save slots later, reintroduce slot IDs and UI.

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

    public Dictionary<string, DialogChoiceData> GetLastChoiceForAllNPCs()
    {
        var result = new Dictionary<string, DialogChoiceData>();

        if (saveData?.choices == null)
            return result;

        for (int i = saveData.choices.Count - 1; i >= 0; i--)
        {
            var choice = saveData.choices[i];
            if (choice == null || string.IsNullOrEmpty(choice.npcName))
                continue;

            // first time we encounter this NPC from the end = latest
            result.TryAdd(choice.npcName, choice);
        }

        return result;
    }

    public string GetLastChoicesSummaryStringForAllNPCs(bool includeQuestion = false)
    {
        var lastChoices = GetLastChoiceForAllNPCs();

        if (lastChoices == null || lastChoices.Count == 0)
            return "No saved choices found.";

        // If you want stable ordering in the output (nice for UI/testing):
        // order by NPC name
        var npcNames = lastChoices.Keys.ToList();
        npcNames.Sort(System.StringComparer.OrdinalIgnoreCase);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Last saved choice per NPC:");

        foreach (var npcName in npcNames)
        {
            var data = lastChoices[npcName];
            if (data == null)
                continue;

            // selectedChoice is what you said you want to display
            sb.AppendLine(includeQuestion
                ? $"- {npcName}: \"{data.selectedChoice}\" (Q: {data.questionText})"
                : $"- {npcName}: \"{data.selectedChoice}\"");
        }

        return sb.ToString();
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