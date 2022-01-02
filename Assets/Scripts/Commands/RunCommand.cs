using System.Collections.Generic;

namespace Commands {

    public class RunCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Starts execution of the program.";

        protected override bool TryExecute (string[] parameters, out string message) {
            GameState.current.running = true;
            message = string.Empty;
            return true;
        }

    }

}
