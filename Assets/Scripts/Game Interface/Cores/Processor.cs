using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Processor : CoreComponent {

        public Processor (Core core, ID id, int slotIndex) : base(core, id, slotIndex) { }

        public override int slotSize => Level.levels[levelIndex].slotSize;

        public void Execute () {
            var taskQueue = TaskQueue.instance;
            var totalSpace = Level.levels[levelIndex].subLevels[upgradeCount].maxTasksPerCycle;
            var spaceRemaining = totalSpace; 
            for(int i=0; i<taskQueue.taskCount; i++){
                if(taskQueue[i].count <= spaceRemaining){
                    var task = taskQueue.TakeTask(i);
                    spaceRemaining -= task.count;
                    task.Execute();
                }
            }
            var usageLevel = 1f - ((float)spaceRemaining / totalSpace);
            onExecute(usageLevel);
        }

        public event System.Action<float> onExecute = delegate {};

        [System.Serializable]
        public class Level {

            public static IReadOnlyList<Level> levels { get; private set; }

            [field: SerializeField] public int slotSize { get; private set; }

            [SerializeField, InlineProperty] private SubLevel [] m_subLevels;

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
            public class SubLevel {

                [field: SerializeField] public int maxTasksPerCycle { get; private set; }

            }

        }

    }

}