using UnityEngine;
using System.Collections.Generic;

namespace Cores.Components {

    [System.Serializable]
    public class Scheduler : CoreComponent {

        public Scheduler (Core core, ID id, int slotIndex) : base(core, id, slotIndex) { }

        public override int slotSize => 1;

        public event System.Action onExecute = delegate {};

        public void Execute () {
            // TODO for(int i... level.taskCount) 
            // this will be the upgrades. they are effective, but they quickly fill the queue with useless tasks
            TaskQueue.instance.TryAddTask(new Tasks.Task(
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

            public static void EnsureLevelsInitialized (Shops.Shop shop) {
                if(levels == null){
                    var list = new List<Level>();
                    foreach(var sched in shop.schedulerPurchases){
                        list.Add(sched.levelData);
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

        }

    }

}