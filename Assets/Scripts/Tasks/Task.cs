using UnityEngine;

namespace Tasks {

    public struct Task {

        // TODO have a central upgrades class that handles that

        // private class LevelData {
        //     public string name;
        //     public int value;
        //     public int purchasePrice;
        // }

        // private static LevelData[] levelData = new LevelData[]{
        //     new LevelData(){name = "Task", value = 1, purchasePrice = 0},
        //     new LevelData(){name = "Super Task", value = 10, purchasePrice = 1000000},
        //     new LevelData(){name = "Hyper Task", value = 1000, purchasePrice = 1000000}
        // }

        // public static int levelCount => levelData.Length;

        [field: SerializeField] public int count { get; private set; }
        [field: SerializeField] public int level { get; private set; }

        public Task (int level, int count) {
            this.level = level;
            this.count = count;
        }

        public void Execute () {
            GameState.current.currency += value;
        }

        public int value { get { return 1; } } // TODO

        public string name { get {
            switch(level){
                case 0: return "Task";
                case 1: return "Super Task";
                case 2: return "Hyper Task";
                default: throw new System.ArgumentException($"Unsupported Task Level \"{level}\"!");
            }
        } }

    }

}