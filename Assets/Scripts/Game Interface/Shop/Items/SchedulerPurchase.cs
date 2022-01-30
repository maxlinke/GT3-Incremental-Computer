using UnityEngine;
using Cores;
using Cores.Components;
using System.Collections.Generic;

namespace Shops.Items {

    [System.Serializable]
    public class SchedulerPurchase : CoreComponentPurchase, IUpgradeSource<Scheduler, Scheduler.Level.Sublevel> {

        [SerializeField] int m_upgradeCost;
        [SerializeField, InlineProperty] Scheduler.Level m_levelData;

        public Scheduler.Level levelData => m_levelData;
        public override string info => $"{levelData.slotSize} Slots\n{levelData.subLevels[0].description}";
        protected override int slotSize => m_levelData.slotSize;

        int IUpgradeSource<Scheduler, Scheduler.Level.Sublevel>.upgradeCost => m_upgradeCost;
        IReadOnlyList<Scheduler.Level.Sublevel> IUpgradeSource<Scheduler, Scheduler.Level.Sublevel>.upgrades => levelData.subLevels;

        protected override void OnPurchased (Core targetCore) {
            targetCore.AddNewScheduler(Scheduler.Level.levels.IndexOf(levelData));
        }

    }

}
