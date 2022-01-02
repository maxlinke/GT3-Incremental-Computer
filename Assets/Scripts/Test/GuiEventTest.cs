using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiEventTest : MonoBehaviour {

    [SerializeField, RedIfEmpty] Text m_text;
    [SerializeField] int m_cacheLength;

    readonly System.Text.StringBuilder m_sb = new System.Text.StringBuilder();
    readonly List<string> m_messages = new List<string>();

    int m_counter;

    void Awake () {
        for(int i=0; i<m_cacheLength; i++){
            m_messages.Add(string.Empty);
        }
    }

    void OnGUI () {
        var curr = Event.current;
        var updateText = false;
        switch(curr.type){
            case EventType.Layout:
            case EventType.Repaint:
                break;
            default:
                m_messages.RemoveAt(0);
                m_messages.Add($"{m_counter} - {Event.current}");
                updateText = true;
                break;
        }
        if(updateText){
            m_sb.Clear();
            foreach(var msg in m_messages){
                m_sb.AppendLine(msg);
            }
            m_text.text = m_sb.ToString();
        }
        m_counter++;
    }

}
