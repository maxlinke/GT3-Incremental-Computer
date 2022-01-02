using UnityEngine;
using UnityEngine.UI;

namespace Cores {

    public class Slot : MonoBehaviour {

        [SerializeField, RedIfEmpty] Image m_image;
        [SerializeField, RedIfEmpty] Text m_text;

        public Color color { set { 
            m_image.color = value;
            m_text.color = value;
        } }

        public string text { set {
            m_text.text = value;
        } }

        public RectTransform rectTransform => (RectTransform)transform;

    }

}