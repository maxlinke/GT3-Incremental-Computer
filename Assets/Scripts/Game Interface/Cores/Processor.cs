namespace Cores.Components {

    [System.Serializable]
    public class Processor : CoreComponent {

        public Processor (Core core, ID id, int slotIndex) : base(core, id, slotIndex) { }

        public override int slotSize { get {
            switch(level){
                case 0: return 1;
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                default: throw new System.ArgumentOutOfRangeException($"Unuspported Processor Level \"{level}\"!");
            }
        } }

        public void Execute () {
            var taskQueue = TaskQueue.instance;
            var totalSpace = 1; // TODO
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

    }

}