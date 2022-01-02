using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Cores {

    public class CoreInfo : MonoBehaviour {

        [SerializeField, RedIfEmpty] Image m_frame;
        [SerializeField, RedIfEmpty] Text m_text;

        public RectTransform rectTransform => (RectTransform)transform;

        Core m_core;

        public void Initialize (Core core) {
            this.m_core = core;
            core.onUnlockStateChanged.Subscribe(OnUnlockStateChanged);
            OnUnlockStateChanged(core.isUnlocked);
        }

        void OnUnlockStateChanged (bool value) {
            var color = Core.GetUnlockStateColor(m_core.isUnlocked);
            m_frame.color = color;
            m_text.color = color;
        }

        void LateUpdate () {
            UpdateText();
        }

        void UpdateText () {
            var temp = Mathf.Clamp(Mathf.RoundToInt(m_core.temperature), 0, 99);
            var speed = Mathf.RoundToInt(100f * m_core.temperatureSpeedFactor);
            m_text.text = $"{temp}°C\n{speed}%";
        }

    }

}