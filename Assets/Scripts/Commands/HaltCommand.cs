using System.Collections.Generic;

namespace Commands {

    public class HaltCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Halts execution of the program.";

        protected override bool TryExecute (string[] parameters, out string message) {
            GameState.current.running = false;
            message = string.Empty;
            return true;
        }

    }

}