using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace GameInterfaceElements.Header {

    public class StatusDisplay : MonoBehaviour {

        [SerializeField, RedIfEmpty] Text m_textField;

        string m_runState;
        bool m_updateDisplay;

        public void Initialize () {
            GameState.onGameStateChanged.Subscribe((gs) => OnRunStateChanged(gs.running));
            GameState.onRunStateChanged.Subscribe(OnRunStateChanged);
            OnRunStateChanged(GameState.current.running);
        }

        void OnRunStateChanged (bool newValue) {
            m_runState = (newValue ? "RUNNING" : "HALTED");
            m_updateDisplay = true;
        }

        void LateUpdate () {
            if(m_updateDisplay){
                UpdateDisplay();
            }
        }

        void UpdateDisplay () {
            m_textField.text = $"{m_runState}";
        }

    }

}