namespace Shops {

    public abstract class UpgradeItem : Item {

        public abstract bool TryPurchase (string targetId, out string errorMessage);

    }

}