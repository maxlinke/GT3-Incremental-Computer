using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tasks;

public class TaskQueue : MonoBehaviour {

    public static TaskQueue instance { get; private set; }

    [SerializeField, RedIfEmpty] VisualTask m_visualTaskTemplate;
    [SerializeField, RedIfEmpty] RectTransform m_visualTaskParent;

    private List<VisualTask> m_activeVisualTasks;
    private Queue<VisualTask> m_pooledVisualTasks;

    public int taskCount => m_activeVisualTasks.Count;
    public Task this[int index] => m_activeVisualTasks[index].task;

    private bool m_updateVisuals;

    public void Initialize () {
        instance = this;
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

        }
    }

}
