using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            var itemName = parameters[0];
            // TODO get the thing from the shop
            message = "TODO implement the rest";
            return false;
        }
    }

}