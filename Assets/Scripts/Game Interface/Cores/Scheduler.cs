namespace Cores.Components {

    [System.Serializable]
    public class Scheduler : CoreComponent {

        public Scheduler (Core core, ID id, int slotIndex) : base(core, id, slotIndex) { }

        public override int slotSize => 1;

        public event System.Action onExecute = delegate {};

        public void Execute () {
            TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: 0,
                count: 1
            ));
            onExecute();
        }

    }

}