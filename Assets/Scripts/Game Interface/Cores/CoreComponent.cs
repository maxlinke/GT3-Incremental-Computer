using UnityEngine;

namespace Cores.Components {

    public abstract class CoreComponent {

        [SerializeField] public ID id;
        [SerializeField] public int level;
        [SerializeField] public int upgrades;

        protected CoreComponent (ID id) {
            this.id = id;
            level = 0;
            upgrades = 0;
        }

        public abstract int slotSize { get ; }

    }

}