namespace Shops {

    public abstract class Item {

        public virtual bool CurrentlyPurchaseable (out string message) {
            if(GameState.current.currency < price){
                var neededAmount = price - GameState.current.currency;
                message = $"Not enough {GameState.CURRENCY_SYMBOL} (need {neededAmount} more).";
                return false;
            }
            message = default;
            return true;
        }

        public virtual string displayName => name;

        public abstract string name { get; }

        public abstract int price { get; }

        public abstract string info { get; }

    }

}