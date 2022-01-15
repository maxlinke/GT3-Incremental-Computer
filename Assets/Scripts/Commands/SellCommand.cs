using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Commands {

    public class SellCommand : Command {

        public override string description => "Sell a component. Only the price of the component itself is returned, any upgrades won't be reimbursed.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "componentId";
        } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = "Please specify the id of the component you wish to sell!";
                return false;
            }
            return ShopDisplay.OnSellCommand(parameters[0], out message);
        }

    }

}
