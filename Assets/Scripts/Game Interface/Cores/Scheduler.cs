using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Scheduler : CoreComponent, IUpgradeable<Scheduler.Level.Sublevel> {

        public override int slotSize => level.slotSize;

        public Level level => Level.levels[levelIndex];

        public event System.Action onExecute = delegate {};
        public event System.Action<int> onUpgrade = delegate {};

        [field: System.NonSerialized] public bool actuallyAddedTasks { get; private set; } = false;

        int IUpgradeable<Level.Sublevel>.currentUpgradeLevel => upgradeCount;

        public void Execute () {
            actuallyAddedTasks = TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: GameState.current.taskLevel,
                count: Level.levels[this.levelIndex].subLevels[this.upgradeCount].taskStackSize
            ));
            onExecute();
        }

        void IUpgradeable<Level.Sublevel>.Upgrade () {
            upgradeCount++;
            onUpgrade(upgradeCount);
        }

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }
            [field: SerializeField, InlineProperty] public SchedulerView.ViewInitData viewInitData { get; private set; }
            [SerializeField, InlineProperty] private Sublevel[] m_subLevels;

            public IReadOnlyList<Sublevel> subLevels => m_subLevels;

            public static void EnsureLevelsInitialized (IEnumerable<Level> inputLevels) {
                levels = levels ?? new List<Level>(inputLevels);
            }

            [System.Serializable]
            public class Sublevel : IUpgrade {

                [field: SerializeField] public int taskStackSize { get; private set; }

                public string description => $"Task-Stack: {taskStackSize}";

            }

        }

    }

}