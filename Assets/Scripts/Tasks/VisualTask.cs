using UnityEngine;
using UnityEngine.UI;

namespace Tasks {

    public class VisualTask : MonoBehaviour {

        [SerializeField, RedIfEmpty] Text m_mainText;
        [SerializeField, RedIfEmpty] Text m_countText;

        public Task task { get; set; }

        public RectTransform rectTransform => (RectTransform)transform;

        public void UpdateText () {
            m_mainText.text = task.name;
            m_countText.text = (task.count > 1 ? $"x{task.count}" : string.Empty);
        }

    }

}