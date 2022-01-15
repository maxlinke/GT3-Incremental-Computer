using UnityEngine;

namespace Cores.Components {

    public abstract class CoreComponent {

        [SerializeField] public ID id;
        [SerializeField] public int slotIndex;
        [SerializeField] public int levelIndex;
        [SerializeField] public int upgradeCount;

        [field: System.NonSerialized] public Core core { get; private set; }
        
        private bool m_initialized;

        public void PostDeserializeInit (Core core) {
            if(m_initialized){
                Debug.LogError($"{this} is already initialized!");
                return;
            }
            if(core == null){
                Debug.LogError($"Null core when initializing {this}");
                return;
            }
            this.core = core;
        }

        protected CoreComponent (Core core, ID id, int slotIndex) {
            this.core = core;
            this.id = id;
            this.slotIndex = slotIndex;
            levelIndex = 0;
            upgradeCount = 0;
            m_initialized = true;
        }

        public abstract int slotSize { get ; }

        public override string ToString () {
            return $"{this.GetType().Name} \"{this.id}\"";
        }

    }

}