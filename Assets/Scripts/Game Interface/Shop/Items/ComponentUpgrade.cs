namespace Shops.Items {

    public abstract class ComponentUpgrade<T> : UpgradeItem {

        public abstract string targetTypeName { get; }

        public override bool TryPurchase (string targetId, out string errorMessage) {
            if(!CurrentlyPurchaseable(out errorMessage)){
                return false;
            }
            if(string.IsNullOrWhiteSpace(targetId)){
                errorMessage = $"Please specify what you want to upgrade. For components, specify their id.";
                return false;
            }
            if(!TryFindTargetForId(targetId, out var target)){
                errorMessage = $"Found no {targetTypeName} with id \"{targetId}\"!";
                return false;
            }
            GameState.current.currency -= price;
            OnPurchased(target);
            return true;
        }

        protected abstract bool TryFindTargetForId (string targetId, out T target);

        protected abstract void OnPurchased (T target);

    }

    // TODO buy core 0 -> buy core0
    // because upgrade core0
    // upgrade only comes with an id
    // so buy <itemName> <id> => buy <itemName> <id (optional)>
    // because coreunlock would only need the itemname

    // TODO subclass coreupgrade
    // TODO subclass componentupgrade
    //      -> processor, scheduler, ... upgrades. 
    //         those are serialized to the purchases per level
    //         the actual upgrade data lives in the real class again
    //         names get generated after the purchases
    //         proc0*, proc0**, proc0***

    // WAIT
    // upgrade 01 -> 01 is a processor at level 0, so upgrade it to level 1
    // upgrade core0 -> core0 is a core
    // upgrade task
    // so it DOES work!!!

}