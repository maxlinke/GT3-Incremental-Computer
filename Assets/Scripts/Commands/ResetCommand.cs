using System.Collections.Generic;

namespace Commands {

    public class ResetCommand : Command {

        public override string description => "Resets the program to the initial state.";

        protected override IEnumerable<string> parameterNames => default;

        protected override bool TryExecute (string[] parameters, out string message) {
            if(GameState.current.hasRun){
                PopupDialogue.ShowYesNoDialogue(
                    message: "Your current state will be lost, continue?", 
                    onYes: () => { GameState.current = new GameState(); },
                    onNo: () => { }
                );
            }else{
                GameState.current = new GameState();
            }
            message = default;
            return true;
        }

    }

}