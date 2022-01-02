using System.Collections.Generic;

namespace Commands {

    public class LoadCommand : Command {

        public override string description => "Restores a saved state of the program.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "fileName";
        } }

        protected override bool TryExecute(string[] parameters, out string message) {
            var fileName = ((parameters.Length > 0) ? parameters[0] : default);
            if(string.IsNullOrWhiteSpace(fileName)){
                message = "Please specify a proper file name!";
                return false;
            }
            if(!SaveData.TryLoadFromDisk(fileName, out var errorMessage)){
                message = errorMessage;
                return false;
            }
            message = "Game state loaded successfully.";
            return true;
        }

    }

}