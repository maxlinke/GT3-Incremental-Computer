namespace Shops {

    public abstract class BuyItem : Item {
        
        public override string name => m_name;
        
        [System.NonSerialized] string m_name;
        public void SetName (string newName) => m_name = newName;

        public override bool CurrentlyPurchaseable (out string message) {
            if(!IsPurchaseableAtAll(out message)){
                return false;
            }
            return base.CurrentlyPurchaseable(out message);
        }

        public bool TryPurchase (Cores.Core targetCore, out string errorMessage) {
            if(!CurrentlyPurchaseable(out errorMessage)){
                return false;
            }
            if(!PurchaseableForCore(targetCore, out errorMessage)){
                return false;
            }
            GameState.current.currency -= price;
            OnPurchased(targetCore);
            return true;
        }

        protected abstract bool IsPurchaseableAtAll (out string message);

        protected abstract bool PurchaseableForCore (Cores.Core targetCore, out string errorMessage);

        protected abstract void OnPurchased (Cores.Core targetCore);

    }

}
