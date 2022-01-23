using UnityEngine;

namespace Cores.Components {

    public abstract class CoreComponent {

        [SerializeField] public ID id;
        [SerializeField] public int slotIndex;
        [SerializeField] public int levelIndex;
        [SerializeField] public int upgradeCount;

        [field: System.NonSerialized] public Core core { get; private set; }
        
        public void SetCore (Core core) {
            this.core = core;
        }

        public CoreComponent () { }

        public abstract int slotSize { get ; }

        public override string ToString () {
            return $"{this.GetType().Name} \"{this.id}\"";
        }

    }

}