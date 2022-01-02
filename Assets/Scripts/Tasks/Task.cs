using UnityEngine;

namespace Tasks {

    public struct Task {

        [field: SerializeField] public int value { get; private set; }
        [field: SerializeField] public int count { get; private set; }
        [field: SerializeField] public int level { get; private set; }

        public Task (int level, int value, int count) {
            this.level = level;
            this.value = value;
            this.count = count;
        }

        public void Execute () {
            GameState.current.currency += value;
        }

    }

}