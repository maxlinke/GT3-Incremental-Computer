using System.Collections.Generic;

namespace Commands {

    public class UpgradeCommand : Command {

        public override string description => $"Upgrades a given component. Check the shop with \"{Command.shopCommandId}\" to see what's on offer.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "id";
        } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = "No id given!";
                return false;
            }
            var id = parameters[0];
            return ShopDisplay.OnUpgradeCommand(id, out message);
        }
        
    }

}