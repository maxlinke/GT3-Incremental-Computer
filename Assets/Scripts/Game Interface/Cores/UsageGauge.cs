using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class UsageGauge : MonoBehaviour {

        [SerializeField] Image m_fillImage;

        public RectTransform rectTransform => (RectTransform)transform;
        public RectTransform rectTransformParent => (RectTransform)(transform.parent);

        public void ShowValue (float value) {
            m_fillImage.fillAmount = value;
        }

    }

}