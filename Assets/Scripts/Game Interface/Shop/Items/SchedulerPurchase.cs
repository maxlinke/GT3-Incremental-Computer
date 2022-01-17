using UnityEngine;
using Cores;
using Cores.Components;

namespace Shops.Items {

    [System.Serializable]
    public class SchedulerPurchase : CoreComponentPurchase {

        [SerializeField] Scheduler.Level m_levelData;

        public Scheduler.Level levelData => m_levelData;
        public override string info => $"{levelData.slotSize} Slots\nTask-Stack: {levelData.taskStackSize}";
        protected override int slotSize => m_levelData.slotSize;

        protected override void OnPurchased (Core targetCore) {
            targetCore.AddScheduler(Scheduler.Level.LevelIndex(levelData));
        }

    }

}
