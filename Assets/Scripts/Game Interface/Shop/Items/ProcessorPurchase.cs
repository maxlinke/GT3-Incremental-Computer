using UnityEngine;
using Cores;
using Cores.Components;

namespace Shops.Items {

    [System.Serializable] 
    public class ProcessorPurchase : ComponentPurchase {

        [SerializeField] Processor.Level m_actualLevel;

        public Processor.Level levelData => m_actualLevel;
        protected override int slotSize => levelData.slotSize;
        public override string info => $"{levelData.slotSize} Slots\n{levelData.maxTasksPerCycle} Tasks/Cycle";

        protected override void OnPurchased (Core targetCore) {
            targetCore.AddProcessor(Processor.Level.LevelIndex(levelData));
        }

    }

}