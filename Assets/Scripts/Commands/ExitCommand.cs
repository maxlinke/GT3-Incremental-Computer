using UnityEngine;
using System.Collections.Generic;

namespace Commands {

    public class ExitCommand : Command {

        protected override IEnumerable<string> parameterNames => null;

        public override string description => "Terminates the program.";

        protected override bool TryExecute(string[] parameters, out string message) {
            if(GameState.current.hasRun){
                PopupDialogue.ShowYesNoCancelDialogue("Save before closing?", CloseWithSave, Close, DontClose);
            }else{
                Close();
            }
            message = default;
            return true;
        }

        void CloseWithSave () {
            if(!SaveData.TrySaveToDisk(GameManager.AUTOSAVE_FILENAME, isAutoSave: true, out var errorMessage)){
                Debug.LogError(errorMessage);
            }
            Close();
        }

        void Close () {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        void DontClose () {
            // if this is null, the cancel won't appear...
        }

    }

}