using UnityEngine;

namespace Shops.Items {

    public abstract class ComponentPurchase : Item {
        
        [SerializeField] int m_price;
        
        [System.NonSerialized] string m_name;
        public void SetName (string newName) => m_name = newName;

        public override int price => m_price;
        public override string name => m_name;

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