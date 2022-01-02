using System.Collections.Generic;

namespace Commands {

    public class HelpCommand : Command {

        protected override IEnumerable<string> parameterNames { get {
            yield return "commandName";
        } }

        public override string description => "Shows how to use a given command.";

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 1){
                message = $"Please specify which command to give help for. Use \"{Command.listCommandId}\" to get a list of available commands.";
                return false;
            }
            if(!Command.TryGetCommandForId(parameters[0], out var targetCommand)){
                message = $"Command \"{parameters[0]}\" doesn't exist!";
                return false;
            }
            message = $"{targetCommand.exampleCall}\n\n{targetCommand.description}";
            return true;
        }

    }

}