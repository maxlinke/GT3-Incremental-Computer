using UnityEngine;
using Cores;
using Cores.Components;
using System.Collections.Generic;

namespace Shops.Items {

    [System.Serializable] 
    public class ProcessorPurchase : CoreComponentPurchase, IUpgradeSource<Processor, Processor.Level.SubLevel> {

        [SerializeField] int m_upgradeCost;
        [SerializeField, InlineProperty] Processor.Level m_actualLevel;

        public Processor.Level levelData => m_actualLevel;
        protected override int slotSize => levelData.slotSize;
        public override string info => $"{levelData.slotSize} Slots\n{levelData.subLevels[0].description}";

        int IUpgradeSource<Processor, Processor.Level.SubLevel>.upgradeCost => m_upgradeCost;
        IReadOnlyList<Processor.Level.SubLevel> IUpgradeSource<Processor, Processor.Level.SubLevel>.upgrades => levelData.subLevels;

        protected override void OnPurchased (Core targetCore) {
            targetCore.AddNewProcessor(Processor.Level.levels.IndexOf(levelData));
        }

    }

}