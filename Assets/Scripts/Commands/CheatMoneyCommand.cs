using System.Collections.Generic;

namespace Commands {

    public class CheatMoneyCommand : Command {

        public override string description => "Gives you money. For free. Amazing, right?";

        protected override IEnumerable<string> parameterNames { get { yield return "amount"; } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = "How much?";
                return false;
            }
            if(!int.TryParse(parameters[0], out var amount)){
                message = $"\"{parameters[0]}\" is not a valid amount!";
                return false;
            }
            GameState.current.currency += amount;
            message = default;
            return true;
        }

    }

}
