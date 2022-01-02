using System.Collections.Generic;

namespace Commands {

    public class ClearCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Removes all entries from the console.";
        
        protected override bool TryExecute (string[] parameters, out string message) {
            InputConsole.instance.Clear();
            message = string.Empty;
            return true;
        }

    }

}
