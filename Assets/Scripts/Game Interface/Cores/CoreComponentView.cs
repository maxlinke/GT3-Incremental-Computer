using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public abstract class CoreComponentView : MonoBehaviour {

        [SerializeField, RedIfEmpty] protected Text m_text;

        public RectTransform rectTransform => (RectTransform)transform;

        public abstract CoreComponent component { get; }

        public int slotSize => component.slotSize;
        public int slotIndex => component.slotIndex;

    }

    public abstract class CoreComponentView<T> : CoreComponentView where T : CoreComponent {

        public virtual void Initialize (T component) {
            rectTransform.SetHeight(rectTransform.rect.height * component.slotSize);
        }

    }

}