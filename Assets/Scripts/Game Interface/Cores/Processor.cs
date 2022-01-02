using UnityEngine;

namespace Cores {

    [System.Serializable]
    public class Processor {

        [SerializeField] public ID id;
        [SerializeField] public int level;
        [SerializeField] public int upgrades;

        public Processor (ID id) {
            this.id = id;
            level = 0;
            upgrades = 0;
        }

        public int slotSize { get {
            switch(level){
                case 0: return 1;
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                default: throw new System.ArgumentOutOfRangeException($"Unuspported Processor Level \"{level}\"!");
            }
        } }

    }

}