using UnityEngine;
using System.Collections.Generic;

namespace Commands {

    public class ExitCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Terminates the program.";

        protected override bool TryExecute(string[] parameters, out string message) {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
            message = default;
            return true;
        }

    }

}