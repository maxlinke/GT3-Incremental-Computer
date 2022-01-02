using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;

public class GameManager : MonoBehaviour {

    const string autoSaveFileName = "autoSave";

    [SerializeField, RedIfEmpty] InputHandler m_inputHandler;
    [SerializeField, RedIfEmpty] GameInterface m_interface;

    [SerializeField] bool m_editorStartFreshEveryTime;
    [SerializeField] bool m_editorDoAutoSaveOnQuit;
    [SerializeField] int m_debugCurrencyGainPerFrame;

    void Start () {
        if(Application.isEditor && m_editorStartFreshEveryTime){
            // nothing
        }else{
            if(!SaveData.TryLoadFromDisk(autoSaveFileName, out var autoSaveError)){
                Debug.LogWarning(autoSaveError);
            }
        }
        // Application.targetFrameRate = 60;
        InitGameComponents();
        InputConsole.instance.PrintMessage(
            $"Welcome! Type \"{Command.runCommandId}\" to start execution of the program. " + 
            $"Type \"{Command.listCommandId}\" to get a list of all other commands " + 
            $"and \"{Command.helpCommandId}\" to get help with particular commands. " + 
            $"Have fun!\n"
        );
    }

    void Update () {
        if(m_debugCurrencyGainPerFrame != 0){
            GameState.current.currency += m_debugCurrencyGainPerFrame;
        }
    }

    void OnApplicationQuit () {
        SaveData.TrySaveToDisk(autoSaveFileName, isAutoSave: true, out _);
    }

    void InitGameComponents () {
        m_inputHandler.Initialize();
        m_interface.Initialize();
    }

}
