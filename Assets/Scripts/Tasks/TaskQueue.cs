using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Tasks;

public class TaskQueue : MonoBehaviour {

    public static TaskQueue instance { get; private set; }

    [SerializeField, RedIfEmpty] VisualTask m_visualTaskTemplate;
    [SerializeField, RedIfEmpty] RectTransform m_visualTaskParent;

    private List<VisualTask> m_activeVisualTasks;
    private Queue<VisualTask> m_pooledVisualTasks;
    private float m_visTaskHeight;

    public int taskCount => m_activeVisualTasks.Count;
    public Task this[int index] => m_activeVisualTasks[index].task;

    private bool m_updateVisuals;

    public void Initialize () {
        instance = this;
        m_activeVisualTasks = new List<VisualTask>();
        m_pooledVisualTasks = new Queue<VisualTask>();
        SpawnAndPoolVisTasks();
        GameState.onGameStateChanged.Subscribe(OnGameStateChanged);
        OnGameStateChanged(GameState.current);
    }

    void SpawnAndPoolVisTasks () {
        m_visualTaskTemplate.SetGOActive(false);
        m_visTaskHeight = m_visualTaskTemplate.rectTransform.rect.height;
        var count = Mathf.FloorToInt(m_visualTaskParent.rect.height / m_visTaskHeight);
        for(int i=0; i<count; i++){
            m_pooledVisualTasks.Enqueue(Instantiate(m_visualTaskTemplate, m_visualTaskParent));
        }
    }

    void OnGameStateChanged (GameState gameState) {
        for(int i=m_activeVisualTasks.Count-1; i>=0; i--){
            var visTask = m_activeVisualTasks[i];
            m_activeVisualTasks.RemoveAt(i);
            m_pooledVisualTasks.Enqueue(visTask);
        }
        for(int i=0; i<gameState.tasks.Count; i++){
            var visTask = m_pooledVisualTasks.Dequeue();
            visTask.task = gameState.tasks[i];
            m_activeVisualTasks.Add(visTask);
        }
        m_updateVisuals = true;
    }

    public Task TakeTask (int index) {
        var visTask = m_activeVisualTasks[index];
        var output = visTask.task;
        m_activeVisualTasks.RemoveAt(index);
        m_pooledVisualTasks.Enqueue(visTask);
        m_updateVisuals = true;
        return output;
    }

    void LateUpdate () {
        if(m_updateVisuals){
            foreach(var visTask in m_pooledVisualTasks){
                visTask.SetGOActive(false);
            }
            var y = 0f;
            foreach(var visTask in m_activeVisualTasks){
                visTask.SetGOActive(true);
                visTask.rectTransform.SetAnchoredY(y);
                visTask.UpdateText();
                y -= m_visTaskHeight;
            }
        }
    }

    public bool TryAddTask (Task task) {
        if(m_pooledVisualTasks.Count > 0){
            var visTask = m_pooledVisualTasks.Dequeue();
            visTask.task = task;
            m_activeVisualTasks.Add(visTask);
            m_updateVisuals = true;
            return true;
        }
        return false;
    }

}
