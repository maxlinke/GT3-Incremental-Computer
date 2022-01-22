using System.Collections.Generic;

namespace Commands {

    public class BuyCommand : Command {

        public override string description => $"Purchases an item listed in the shop and puts it on the indicated core, if possible. Check the shop with \"{Command.shopCommandId}\" to see what's on offer.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "item";
            yield return "coreIndex (optional)";
        } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = "No paramters given. Please specify an item and possibly the index of the target core.";
                return false;
            }
            var itemName = parameters[0];
            var core = default(Cores.Core);
            if(parameters.Length > 1){
                var coreIndexString = parameters[1];
                if(!GameState.current.TryFindCoreForIndex(coreIndexString, out core)){
                    message = $"\"{coreIndexString}\" is not a valid core index.";
                    return false;
                }
            }
            return ShopDisplay.OnBuyCommand(itemName, core, out message);
        }

    }

}