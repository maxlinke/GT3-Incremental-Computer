using UnityEngine;
using Commands;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour {

    const string CHEAT_COMMAND_ARG = "enable_cheats";
    public const string AUTOSAVE_FILENAME = "autoSave";
    public const int MIN_HOURS_FOR_WELCOME_BACK_BONUS = 6;
    public const float WELCOME_BACK_BONUS_PERCENTAGE = 0.1f;

    [SerializeField, RedIfEmpty] InputHandler m_inputHandler;
    [SerializeField, RedIfEmpty] GameInterface m_interface;
    [SerializeField, RedIfEmpty] PopupDialogue m_popupDialogue;

    void Start () {
        Application.targetFrameRate = 60;
        InitGameComponents();
        if(!GameState.current.hasRun){
            InputConsole.instance.PrintMessage(
                $"Welcome! Type \"{Command.runCommandId}\" to start execution of the program. " + 
                $"Type \"{Command.listCommandId}\" to get a list of all other commands " + 
                $"and \"{Command.helpCommandId}\" to get help with particular commands. " + 
                $"Have fun!\n"
            );
        }
        Commands.Command.moneyCheatEnabled = MoneyCheatShouldBeEnabled();
        if(SaveData.SaveFileExists(AUTOSAVE_FILENAME)){
            PopupDialogue.ShowYesNoDialogue("Load the latest auto-save?", LoadAutoSave, DontLoadAutoSave);
        }
    }

    void LoadAutoSave () {
        if(!SaveData.TryLoadFromDisk(AUTOSAVE_FILENAME, out var errorMsg)){
            Debug.LogError(errorMsg);
        }
    }

    void DontLoadAutoSave () { }

    void InitGameComponents () {
        m_inputHandler.Initialize();
        m_interface.Initialize();
        m_popupDialogue.Initialize();
    }

    bool MoneyCheatShouldBeEnabled () {
#if UNITY_EDITOR
        return true;
#else
        foreach(var cmdArg in System.Environment.GetCommandLineArgs()){
            if(cmdArg.Trim().ToLower().Equals(CHEAT_COMMAND_ARG.ToLower())){
                return true;
            }
        }
        return false;
#endif
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    int taskLevel;
    int taskCount;

    int temperatureTargetCore;
    int temperatureTarget;
    float temperatureImpulseStrength;

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        if(EditorApplication.isPlaying){
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Tasks n shiz", EditorStyles.boldLabel);
            DrawTaskStuff();
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Temperature", EditorStyles.boldLabel);
            DrawTemperatureStuff();
        }
    }

    void DrawTaskStuff () {
        taskLevel = EditorGUILayout.IntSlider("level", taskLevel, 0, 2);
        taskCount = EditorGUILayout.IntSlider("count", taskCount, 1, 1024);
        if(GUILayout.Button("add task")){
            TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: taskLevel, 
                count: taskCount
            ));
        }
        if(GUILayout.Button("take task")){
            if(TaskQueue.instance.taskCount > 0){
                TaskQueue.instance.TakeTask(0);
            }
        }
        if(GUILayout.Button("add random")){
            TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: UnityEngine.Random.Range(0, 3), 
                count: UnityEngine.Random.Range(1, 1025)
            ));
        }
        if(GUILayout.Button("take random")){
            if(TaskQueue.instance.taskCount > 0){
                TaskQueue.instance.TakeTask(UnityEngine.Random.Range(0, TaskQueue.instance.taskCount));
            }
        }
    }

    void DrawTemperatureStuff () {
        temperatureTargetCore =      EditorGUILayout.IntField("temperatureTargetCore", temperatureTargetCore);
        temperatureTarget =          EditorGUILayout.IntField("temperatureTarget", temperatureTarget);
        temperatureImpulseStrength = EditorGUILayout.Slider("temperatureImpulseStrength", temperatureImpulseStrength, 0, 1);
        if(GUILayout.Button("do temperature")){
            if(GameState.current.TryFindCoreForIndex(temperatureTargetCore, out var core)){
                core.AddTemperatureImpulse(new Cores.Core.TemperatureImpulse(){
                    targetTemperature = temperatureTarget, 
                    impulseStrength = temperatureImpulseStrength
                });
            }
        }
    }

}
#endif
