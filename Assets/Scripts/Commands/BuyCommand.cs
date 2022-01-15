using System.Collections.Generic;

namespace Commands {

    public class BuyCommand : Command {

        public override string description => $"Purchases an item listed in the shop and puts it on the indicated core, if possible. Check the shop with \"{Command.shopCommandId}\" to see what's on offer.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "item";
            yield return "coreIndex";
        } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = "No paramters given. Please specify an item and the index of the target core.";
                return false;
            }
            if(parameters.Length < 2){
                message = "No core index given, please also specify the you want to purchase the item for.";
                return false;
            }
            var itemName = parameters[0];
            var coreIndexString = parameters[1];
            var goodParse = int.TryParse(coreIndexString, out var coreIndex);
            var core = default(Cores.Core);
            if(goodParse){
                try{
                    core = GameState.current.cores[coreIndex];
                }catch(System.IndexOutOfRangeException){
                    core = default;
                }catch(System.ArgumentOutOfRangeException){
                    core = default;
                }
            }
            if(!goodParse || core == default){
                message = $"\"{coreIndexString}\" is not a valid core index.";
                return false;
            }
            return ShopDisplay.OnBuyCommand(itemName, core, out message);
        }

    }

}