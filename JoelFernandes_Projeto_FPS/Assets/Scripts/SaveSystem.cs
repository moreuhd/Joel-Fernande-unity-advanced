

using System.IO;
using UnityEngine;
using UnityEngine.Events;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/savegame.json";
    public static UnityEvent<GameData> onSave = new UnityEvent<GameData>();
    public static UnityEvent<GameData> onLoad = new UnityEvent<GameData>();
    private static GameData _gameData = new GameData();

    public static void SaveGame()
    {
        onSave?.Invoke(_gameData);
        string json = JsonUtility.ToJson(_gameData, true);
        File.WriteAllText(savePath, json);
        
    }

    public static void LoadGame()
    {
        if (File.Exists(savePath))
        {
            
            string json = File.ReadAllText(savePath);
            _gameData = JsonUtility.FromJson<GameData>(json);
            onLoad?.Invoke(_gameData);
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }
    }
}

