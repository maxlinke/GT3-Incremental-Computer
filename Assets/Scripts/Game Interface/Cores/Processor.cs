using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Processor : CoreComponent, IUpgradeable<Processor.Level.SubLevel> {

        public override int slotSize => Level.levels[levelIndex].slotSize;

        public event System.Action<float> onExecute = delegate {};
        public event System.Action<int> onUpgraded = delegate {};
        
        int IUpgradeable<Level.SubLevel>.currentUpgradeLevel => upgradeCount;

        void IUpgradeable<Level.SubLevel>.Upgrade () {
            upgradeCount++;
            onUpgraded(upgradeCount);
        }

        public void Execute () {
            var taskQueue = TaskQueue.instance;
            var currentLevel = Level.levels[levelIndex];
            var totalSpace = currentLevel.subLevels[upgradeCount].maxTasksPerCycle;
            var spaceRemaining = totalSpace; 
            for(int i=0; i<taskQueue.taskCount; i++){
                if(taskQueue[i].count <= spaceRemaining){
                    var task = taskQueue.TakeTask(i);
                    i--;
                    spaceRemaining -= task.count;
                    task.Execute();
                }
            }
            var usageLevel = 1f - ((float)spaceRemaining / totalSpace);
            var heatImpulse = new Core.TemperatureImpulse(){
                targetTemperature = GameState.DEFAULT_TEMPERATURE + currentLevel.temperatureDelta,
                impulseStrength = currentLevel.heatImpulseStrength * usageLevel
            };
            core.AddTemperatureImpulse(heatImpulse);
            onExecute(usageLevel);
        }

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }
            [field: SerializeField] public float temperatureDelta { get; private set; }
            [field: SerializeField] public float heatImpulseStrength { get; private set; }
            [SerializeField, InlineProperty] private SubLevel[] m_subLevels;

            public IReadOnlyList<SubLevel> subLevels => m_subLevels;

            public static void EnsureLevelsInitialized (IEnumerable<Level> inputLevels) {
                levels = levels ?? new List<Level>(inputLevels);
            }

            [System.Serializable]
            public class SubLevel : IUpgrade {

                [field: SerializeField] public int maxTasksPerCycle { get; private set; }

                public string description => $"{maxTasksPerCycle} Tasks/Cycle";
            }

        }

    }

}