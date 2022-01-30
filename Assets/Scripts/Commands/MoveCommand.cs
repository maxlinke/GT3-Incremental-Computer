using System.Collections.Generic;

namespace Commands {

    public class MoveCommand : Command {

        public override string description => "Moves the targeted component to a specified core, if possible.";

        protected override IEnumerable<string> parameterNames { get {
            yield return "componentId";
            yield return "coreIndex";
        } }

        protected override bool TryExecute (string[] parameters, out string message) {
            if(parameters.Length < 2){
                message = "Please specify the component and target core!";
                return false;
            }
            var componentId = parameters[0];
            var coreIndex = parameters[1];
            if(!GameState.current.TryFindComponentForId(componentId, out var targetComponent)){
                message = $"There is no component with id \"{componentId}\"!";
                return false;
            }
            if(!GameState.current.TryFindCoreForIndex(coreIndex, out var targetCore)){
                message = $"There is no core with index \"{coreIndex}\"!";
                return false;
            }
            if(!targetCore.unlocked){
                message = "Target core isn't unlocked!";
                return false;
            }
            if(!(targetCore.remainingSlots >= targetComponent.slotSize)){
                message = "Target core doesn't have enough space!";
                return false;
            }
            targetComponent.core.RemoveComponent(targetComponent);
            targetCore.AddComponent(targetComponent);
            message = string.Empty;
            return true;
        }

    }

}