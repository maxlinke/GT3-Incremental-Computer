namespace Shops {

    public abstract class Item {

        public bool CurrentlyPurchaseable (out string message) {
            if(!IsPurchaseableAtAll(out message)){
                return false;
            }
            if(GameState.current.currency < price){
                message = $"Not enough {GameState.CURRENCY_SYMBOL}.";
                return false;
            }
            message = default;
            return true;
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

        public abstract bool IsPurchaseableAtAll (out string message);

        protected abstract bool PurchaseableForCore (Cores.Core targetCore, out string errorMessage);

        protected abstract void OnPurchased (Cores.Core targetCore);

        public abstract string name { get; }

        public abstract int price { get; }

        public abstract string info { get; }

    }

}