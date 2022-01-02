using System.Collections.Generic;

namespace Commands {

    public class ListCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Lists all available commands.";

        protected override bool TryExecute (string[] parameters, out string message) {
            var sb = new System.Text.StringBuilder();
            foreach(var command in Command.allCommands){
                sb.AppendLine(command.id);
            }
            message = sb.ToString();
            if(string.IsNullOrEmpty(message)){
                return false;
            }
            message = $"Available commands: \n\n{message}";
            return true;
        }

    }

}