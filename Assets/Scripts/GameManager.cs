using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Commands;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameManager : MonoBehaviour {

    const string autoSaveFileName = "autoSave";

    [SerializeField, RedIfEmpty] InputHandler m_inputHandler;
    [SerializeField, RedIfEmpty] GameInterface m_interface;

    [SerializeField] int m_debugCurrencyGainPerFrame;

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
    }

    void Update () {
        if(m_debugCurrencyGainPerFrame != 0){
            GameState.current.currency += m_debugCurrencyGainPerFrame;
        }
    }

    void InitGameComponents () {
        m_inputHandler.Initialize();
        m_interface.Initialize();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor {

    int taskLevel;
    int taskCount;

    public override void OnInspectorGUI () {
        base.OnInspectorGUI();
        if(EditorApplication.isPlaying){
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Tasks n shiz", EditorStyles.boldLabel);
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
                    level: Random.Range(0, 3), 
                    count: Random.Range(1, 1025)
                ));
            }
            if(GUILayout.Button("take random")){
                if(TaskQueue.instance.taskCount > 0){
                    TaskQueue.instance.TakeTask(Random.Range(0, TaskQueue.instance.taskCount));
                }
            }
        }
    }

}
#endif
