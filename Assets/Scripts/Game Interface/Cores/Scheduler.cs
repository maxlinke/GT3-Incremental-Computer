using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Scheduler : CoreComponent {

        public override int slotSize => 1;

        public Level level => Level.levels[levelIndex];

        public event System.Action onExecute = delegate {};

        [field: System.NonSerialized] public bool actuallyAddedTasks { get; private set; } = false;

        public void Execute () {
            // TODO for(int i... level.taskCount) 
            // this will be the upgrades. they are effective, but they quickly fill the queue with useless tasks
            actuallyAddedTasks = TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: GameState.current.taskLevel,
                count: Level.levels[this.levelIndex].taskStackSize
            ));
            onExecute();
        }

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }
            [field: SerializeField] public int taskStackSize { get; private set; }
            [field: SerializeField, InlineProperty] public SchedulerView.ViewInitData viewInitData { get; private set; }

            public static void EnsureLevelsInitialized (IEnumerable<Level> inputLevels) {
                levels = levels ?? new List<Level>(inputLevels);
            }

        }

    }

}