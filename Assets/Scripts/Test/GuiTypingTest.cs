using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GuiTypingTest : MonoBehaviour {

    // TODO see if i can make this with unirx. less if and else and such
    // TODO tabbing NOT as a character but as a "prediction"
    // TODO up-arrowing to go through the history
    // TODO some form of cursor? maybe? how much do i need text editing features here?

    // />buy worker2
    // />move 39a4 proc2
    // />sell 9bf3
    // />upgrade 492f
    // />select p0
    // p0/>select w4
    // p0/w4/>upgrade
    // p0/w4/>select null
    // />select p2/w0
    // p2/w0/> select 0
    // />select 12be
    // p7/d2/> select 0
    // />clear

    // probably ok to not have fancy text editing
    // escape to cancel the entry is probably enough
    // just need to know when the "selection" changes via mouse click to auto update the console

    [SerializeField, RedIfEmpty] Text m_inputText;
    [SerializeField, RedIfEmpty] Text m_outputText; 

    readonly System.Text.StringBuilder m_buffer = new System.Text.StringBuilder();

    readonly Subject<Event> m_onKeyEvent = new UniRx.Subject<Event>();

    void Awake () {
        m_inputText.text = string.Empty;
        m_outputText.text = string.Empty;
        InitObservables();
    }

    // TODO OBVIOUSLY NOT!!!
    void Update () {
        UpdateInputText();
    }

    void InitObservables () {
        m_onKeyEvent
            .Select(evt => GetSpecialEventHandler(evt))
            .Where(handler => handler != default)
            .Subscribe(handler => {
                handler.execute();
                handler.evt.Use();
            });
        m_onKeyEvent
            .Where(evt => IsTextCharacter(evt.character))
            .Subscribe(evt => {
                TextEntered(evt.character);
                evt.Use();
            });
    }

    bool IsTextCharacter (char character) {
        return character >= 32 && character <= 127;
    }

    (Event evt, System.Action execute) GetSpecialEventHandler (Event evt) {
        if((evt.modifiers & EventModifiers.Control) == EventModifiers.Control){
            switch(evt.keyCode){
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                    return default;
                case KeyCode.C:
                    return (evt, HandleCancelInput);
            }
            var keyCodeString = evt.keyCode.ToString();
            if(keyCodeString.Length == 1){
                var keyCodeChar = keyCodeString[0];
                if(Char.IsLetterOrDigit(keyCodeChar)){
                    return (evt, () => HandleUnknownCtrlEvent(Char.ToUpper(keyCodeChar)));
                }
            }
        }
        switch(evt.keyCode){
            case KeyCode.Return:
                return (evt, HandleReturn);
            case KeyCode.Backspace:
                return (evt, HandleBackspace);
            default: 
                return default;
        }
    }

    void HandleCancelInput () {
        Debug.Log("Ctrl+C pressed!");
    }

    void HandleUnknownCtrlEvent (char character) {
        Debug.Log($"Unknown Command Ctrl+{character}!");
    }

    void TextEntered (char letter) {
        if(m_buffer.Length < 32){
            m_buffer.Append(letter);
        }
    }

    void HandleBackspace () {
        if(m_buffer.Length > 0){
            m_buffer.Remove(m_buffer.Length - 1, 1);
        }
    }

    void HandleReturn () {
        m_outputText.text += $"\n{m_buffer.ToString()}";
        m_buffer.Clear();
    }

    void OnGUI () {
        var curr = Event.current;
        if(curr.type == EventType.KeyDown){
            m_onKeyEvent.OnNext(curr);
            // if(curr.character != 0){
            //     if(curr.character == 10){
            //         m_outputText.text += $"\n{m_buffer.ToString()}";
            //         m_buffer.Clear();
            //     }else{
            //         m_buffer.Append(curr.character);
            //     }
            //     UpdateInputText();
            //     Event.current.Use();
            // }else if((curr.keyCode == KeyCode.Backspace) && (m_buffer.Length > 0)){
            //     m_buffer.Remove(m_buffer.Length - 1, 1);
            //     UpdateInputText();
            //     Event.current.Use();
            // }
        }
    }

    void UpdateInputText () => m_inputText.text = m_buffer.ToString();

}
