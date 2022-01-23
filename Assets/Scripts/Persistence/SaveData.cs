using UnityEngine;
using System.IO;

[System.Serializable]
public class SaveData {

    const int CURRENT_VERSION = 5;

    private static string saveFolderPath => $"{Application.persistentDataPath}/Saves";

    public static string GetFilePath (string fileName) {
        return $"{saveFolderPath}/{fileName}.json";
    }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Save Data/Open Save Directory")]
    private static void OpenSaveDirectory () {
        try{
            if(!System.IO.Directory.Exists(saveFolderPath)){
                System.IO.Directory.CreateDirectory(saveFolderPath);
            }
            System.Diagnostics.Process.Start(saveFolderPath);
        }catch(System.Exception e){
            Debug.LogException(e);
        }
    }
#endif

    [SerializeField] int m_version;
    [SerializeField] bool m_isAutoSave;
    [SerializeField] long m_timeStampBinary;

    [SerializeField] GameState m_gameState;
    
    private SaveData (bool isAutoSave) {
        m_isAutoSave = isAutoSave;
        m_timeStampBinary = System.DateTime.Now.ToBinary();
        m_version = CURRENT_VERSION;
        m_gameState = GameState.current;
    }

    public static bool TrySaveToDisk (string fileName, bool isAutoSave, out string errorMessage) {
        try{
            var saveData = new SaveData(isAutoSave);
            var json = JsonUtility.ToJson(saveData);
            File.WriteAllText(SaveData.GetFilePath(fileName), json);
            errorMessage = default;
            return true;
        }catch(System.Exception e){
            Debug.LogException(e);
            errorMessage  = $"ERROR: {e.GetType()}";
            return false;
        }
    }

    public static bool SaveFileExists (string fileName) {
        return File.Exists(GetFilePath(fileName));
    }

    public static bool TryLoadFromDisk (string fileName, out string errorMessage) {
        try{
            if(!SaveFileExists(fileName)){
                errorMessage = $"There is no save file named \"{fileName}\"!";
                return false;
            }
            var json = File.ReadAllText(GetFilePath(fileName));
            var saveData = JsonUtility.FromJson<SaveData>(json);
            if(!saveData.TryApply(out errorMessage)){
                return false;
            }
            errorMessage = default;
            return true;
        }catch(System.Exception e){
            Debug.LogException(e);
            errorMessage = $"ERROR: {e.GetType()}";
            return false;
        }
    }

    private bool TryApply (out string errorMessage) {
        if(this.m_version != CURRENT_VERSION){
            errorMessage = "Version mismatch, cannot set game state!";
            return false;
        }
        try{
            GameState.current = m_gameState;
            if(this.m_isAutoSave){
                if(GameState.current.running){
                    var lastSaveTime = System.DateTime.FromBinary(m_timeStampBinary);
                    var timeSpan = System.DateTime.Now - lastSaveTime;
                    // TODO give (a little bit) of currency, depending on the processor output last time
                    Debug.Log($"TODO this\n{timeSpan}");
                }
            }
            errorMessage = default;
            return true;
        }catch(System.Exception e){
            Debug.LogException(e);
            errorMessage = $"Error: {e.GetType()}";
            return false;
        }
    }

}
