using System.Collections.Generic;

namespace Commands {

    public abstract class Command {

        public const string runCommandId = "run";
        public const string listCommandId = "list";
        public const string helpCommandId = "help";
        public const string shopCommandId = "shop";

        public const string buyCommandId = "buy";
        public const string upgradeCommandId = "upgrade";

        private static List<Command> commands = new List<Command>(){
            new RunCommand(){id = runCommandId},
            new HaltCommand(){id = "halt"},
            new ClearCommand(){id = "clear"},
            new ListCommand(){id = listCommandId},
            new HelpCommand(){id = helpCommandId},
            new ShopCommand(){id = shopCommandId},
            new BuyCommand(){id = buyCommandId},
            new SellCommand(){id = "sell"},
            // move
            new SaveCommand(){id = "save"},
            new LoadCommand(){id = "load"},
            new ExitCommand(){id = "exit"},
        };

        public static IEnumerable<Command> allCommands => commands;

        protected Command () { }

        public string id { get; private set; }

        protected abstract IEnumerable<string> parameterNames { get; }
        public string exampleCall { get {
            var output = id;
            var anyParams = false;
            if(parameterNames != null){
                foreach(var paramName in parameterNames){
                    output += $" <{paramName}>";
                    anyParams = true;
                }
            }
            if(!anyParams){
                output += " (no parameters)";
            }
            return output;
        } }

        public abstract string description { get; }

        protected abstract bool TryExecute (string[] parameters, out string message);

        public static bool TryGetCommandForId (string cmdId, out Command output) {
            output = commands.Find((cmd) => (cmd.id == cmdId));
            return output != null;
        }

        public static bool TryExecute (string input, out string message) {
            if(string.IsNullOrWhiteSpace(input)){
                message = "No input given";
                return false;
            }
            var inputs = input.Trim().Split((char[])null, System.StringSplitOptions.RemoveEmptyEntries);
            var cmdId = inputs[0].ToLower();
            if(!TryGetCommandForId(cmdId, out var cmd)){
                message = $"Unknown command \"{inputs[0]}\"";
                return false;
            }
            var parameters = new string[inputs.Length-1];
            for(int i=0; i<parameters.Length; i++){
                parameters[i] = inputs[i+1];
            }
            var executedSuccessfully = cmd.TryExecute(parameters, out message);
            message = message ?? string.Empty;
            return executedSuccessfully;
        }

    }

}