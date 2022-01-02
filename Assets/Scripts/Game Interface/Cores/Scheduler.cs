namespace Cores.Components {

    [System.Serializable]
    public class Scheduler : CoreComponent {

        public Scheduler (ID id) : base(id) { }

        public override int slotSize => 1;

        public void Execute () {
            TaskQueue.instance.TryAddTask(new Tasks.Task(
                level: 0,
                count: 1
            ));
        }

    }

}