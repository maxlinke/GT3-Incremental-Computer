using UnityEngine;
using Cores;
using Cores.Components;

namespace Shops.Items {

    [System.Serializable] 
    public class ProcessorPurchase : ComponentPurchase {

        [SerializeField] int m_price;
        [SerializeField] Processor.Level m_actualLevel;

        public override string name => m_name;
        public override int price => m_price;
        public override string info => $"{levelData.slotSize} Slots\n{levelData.maxTasksPerCycle} Tasks/Cycle";
        public Processor.Level levelData => m_actualLevel;

        protected override int slotSize => levelData.slotSize;

        [System.NonSerialized] string m_name;
        public void SetName (string newName) => m_name = newName;

        protected override void OnPurchased (Core targetCore) {
            targetCore.AddProcessor(Processor.Level.LevelIndex(levelData));
        }

    }

}