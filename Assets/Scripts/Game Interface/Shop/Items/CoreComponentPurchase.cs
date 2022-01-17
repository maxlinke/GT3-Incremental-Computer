using UnityEngine;

namespace Shops.Items {

    public abstract class CoreComponentPurchase : BuyItem {
        
        [SerializeField] int m_price;

        public override string displayName => $"{name} <core>";

        public override int price => m_price;

        protected abstract int slotSize { get; }

        protected override bool IsPurchaseableAtAll (out string message) { 
            message = default;
            return true;
        }

        protected override bool PurchaseableForCore (Cores.Core targetCore, out string errorMessage) {
            if(targetCore == default){
                errorMessage = "No core specified!";
                return false;
            }
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