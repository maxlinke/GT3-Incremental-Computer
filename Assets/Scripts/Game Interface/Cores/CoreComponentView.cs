using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public abstract class CoreComponentView : MonoBehaviour {

        public RectTransform rectTransform => (RectTransform)transform;

        public abstract CoreComponent component { get; }

        public abstract int slotSize { get; }

        public abstract int slotIndex { get; }

    }

    public abstract class CoreComponentView<T> : CoreComponentView where T : CoreComponent {

        [SerializeField, RedIfEmpty] protected Text m_text;

        public abstract void Initialize (T value);

        public override int slotSize => component.slotSize;

        public override int slotIndex => component.slotIndex;

    }

}