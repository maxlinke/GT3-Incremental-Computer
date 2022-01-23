using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Processor : CoreComponent, IUpgradeable<Processor.Level.SubLevel> {

        public Processor (Core core, ID id, int slotIndex) : base(core, id, slotIndex) { }

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
            core.AddTemperatureImpulse(currentLevel.cycleTemperatureImpulseTarget, currentLevel.cycleTemperatureImpulseStrength * usageLevel);
            onExecute(usageLevel);
        }

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }
            [field: SerializeField] public float cycleTemperatureImpulseTarget { get; private set; }
            [field: SerializeField] public float cycleTemperatureImpulseStrength { get; private set; }
            [SerializeField, InlineProperty] private SubLevel[] m_subLevels;

            public IReadOnlyList<SubLevel> subLevels => m_subLevels;

            public static void EnsureLevelsInitialized (Shops.Shop shop) {
                if(levels == null){
                    var list = new List<Level>();
                    foreach(var proc in shop.processorPurchases){
                        list.Add(proc.levelData);
                    }
                    levels = list;
                }
            }

            public static int LevelIndex (Level level) {
                for(int i=0; i<levels.Count; i++){
                    if(levels[i] == level){
                        return i;
                    }
                }
                return -1;
            }

            [System.Serializable]
            public class SubLevel : IUpgrade {

                [field: SerializeField] public int maxTasksPerCycle { get; private set; }

                public string description => $"{maxTasksPerCycle} Tasks/Cycle";
            }

        }

    }

}