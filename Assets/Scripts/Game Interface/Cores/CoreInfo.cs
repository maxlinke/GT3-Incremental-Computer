using UnityEngine;
using UnityEngine.UI;

namespace Cores {

    public class CoreInfo : MonoBehaviour {

        [SerializeField, RedIfEmpty] Image m_frame;
        [SerializeField, RedIfEmpty] Text m_text;

        public RectTransform rectTransform => (RectTransform)transform;

        CoreView m_coreView;

        public void Initialize (CoreView coreView) {
            this.m_coreView = coreView;
            coreView.onUnlockStateChanged += OnUnlockStateChanged;
            OnUnlockStateChanged(coreView.core.unlocked);
        }

        void OnUnlockStateChanged (bool value) {
            var color = CoreView.GetUnlockStateColor(value);
            m_frame.color = color;
            m_text.color = color;
        }

        void LateUpdate () {
            UpdateText();
        }

        void UpdateText () {
            string temp;
            if(m_coreView.core.temperature < -9 || m_coreView.core.temperature > 99){
                temp = "ERR";
            }else{
                temp = $"{Mathf.RoundToInt(m_coreView.core.temperature)}°C";
            }
            string speed = $"{Mathf.RoundToInt(100f * m_coreView.core.temperatureSpeedFactor)}%";
            m_text.text = $"{temp}\n{speed}";
        }

    }

}