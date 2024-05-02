using System.Collections.Generic;

namespace Commands {

    public class SaveCommand : Command {

        public override string description => "Saves the state of the program so it can be loaded again later.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "fileName"; 
        } }

        protected override bool TryExecute(string[] parameters, out string message) {
#if UNITY_WEBGL
            message = "This command is disabled in WebGL builds";
            return false;
#endif
            var fileName = ((parameters.Length > 0) ? parameters[0] : default);
            if(string.IsNullOrWhiteSpace(fileName)){
                message = "Please specify a proper file name!";
                return false;
            }
            if(!SaveData.TrySaveToDisk(fileName, isAutoSave: false, out var errorMessage)){
                message = errorMessage;
                return false;
            }
            message = "Game state saved successfully";
            return true;
        }

    }

}