using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public abstract class VisualCoreComponent : MonoBehaviour {

        public RectTransform rectTransform => (RectTransform)transform;

        public abstract int slotSize { get; }

    }

    public abstract class VisualCoreComponent<T> : VisualCoreComponent {

        [SerializeField, RedIfEmpty] protected Text m_text;

        public abstract void Initialize (T value);

        public abstract void Execute (float expectedDuration);

    }

}