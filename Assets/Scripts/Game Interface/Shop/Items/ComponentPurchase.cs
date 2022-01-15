namespace Shops.Items {

    public abstract class ComponentPurchase : Item {

        protected abstract int slotSize { get; }

        public override bool IsPurchaseableAtAll (out string message) { 
            message = default;
            return true;
        }

        protected override bool PurchaseableForCore (Cores.Core targetCore, out string errorMessage) {
            if(!targetCore.unlocked){
                errorMessage = $"Core {targetCore.index} isn't unlocked yet!";
                return false;
            }
            if(targetCore.remainingSlots < slotSize){
                errorMessage = $"Core {targetCore.index} doesn't have enough free slots, sell or move components to make room!";
                return false;
            }
            errorMessage = default;
            return true;
        }

    }

}