using System.Collections.Generic;
using Cores;

namespace Shops.Items {

    public class CoreUnlock : Item {

        public static void EnsureListInitialized (Shop shop) {
            if(allUnlocks != null){
                return;
            }
            var output = new List<CoreUnlock>();
            for(int i=0; i<CoreDisplay.NUMBER_OF_CORES; i++){
                output.Add(new CoreUnlock(shop, i));
            }
            allUnlocks = output;
        }

        public static IReadOnlyList<CoreUnlock> allUnlocks { get; private set; }

        public override string name => m_name;
        public override int price => m_price;

        public override bool IsPurchaseableAtAll (out string message) {
            return PurchaseableForCore(GameState.current.cores[m_coreIndex], out message);
        }

        protected override bool PurchaseableForCore (Core targetCore, out string errorMessage) {
            if(GameState.current.cores[m_coreIndex].unlocked){
                errorMessage = $"Core {m_coreIndex} is already unlocked.";
                return false;
            }
            errorMessage = default;
            return true;
        }

        protected override void OnPurchased (Core targetCore) {
            targetCore.Unlock();
        }

        private readonly int m_coreIndex;
        private readonly string m_name;
        private readonly int m_price;

        private CoreUnlock (Shop shop, int index) {
            this.m_coreIndex = index;
            this.m_name = $"{Shop.CMD_CAT_CORE} {index}";
            if(index == 0){
                this.m_price = 0;
            }else{                    
                this.m_price = shop.firstUnlockableCoreCost;
                for(int i=1; i<index; i++){
                    this.m_price *= shop.eachNewUnlockableCoreCostFactor;
                }
            }
        }

    }

}
