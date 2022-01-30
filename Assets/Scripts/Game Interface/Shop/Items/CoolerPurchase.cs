using UnityEngine;
using Cores;
using Cores.Components;

namespace Shops.Items {

    [System.Serializable]
    public class CoolerPurchase : CoreComponentPurchase {

        [SerializeField] string m_info;
        [SerializeField, InlineProperty] Cooler.Level m_levelData;

        public Cooler.Level levelData => m_levelData;
        public override string info => m_info;
        protected override int slotSize => m_levelData.slotSize;

        protected override void OnPurchased(Core targetCore) {
            targetCore.AddNewCooler(Cooler.Level.levels.IndexOf(levelData));
        }

    }

}
