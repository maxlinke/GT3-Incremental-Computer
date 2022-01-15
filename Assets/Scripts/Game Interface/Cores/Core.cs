using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cores.Components;

namespace Cores {

    [System.Serializable]
    public class Core : ISerializationCallbackReceiver {

        public const int SLOT_COUNT = 6;
        public const float DEFAULT_TEMPERATURE = 21;
        private const float DEFAULT_TEMPERATURE_SPEED = 1;

        public const bool START_ON_PROCESSOR_CYCLE = false;

        public const float TEMP_CYCLES_PER_SECOND = 2f;

        public static Core GetInitialCore (System.Func<ID> getNewId) {
            var output = new Core();
            output.unlocked = true;
            output.m_processors.Add(new Processor(output, getNewId(), output.nextFreeSlotIndex));
            output.m_schedulers.Add(new Scheduler(output, getNewId(), output.nextFreeSlotIndex));
            return output;
        }

        public static int UnlockCost (int coreIndex) {
            if(coreIndex < 1){
                return 0;
            }
            int output = 100;
            while(coreIndex > 1){
                output *= 10;
            }
            return output;
        }

        [field: SerializeField] public bool unlocked { get; private set; }
        [field: SerializeField] public bool running { get; private set; }
        [field: SerializeField] public float temperature { get; private set; }
        [field: SerializeField] public float cycleTimer { get; private set; }
        [field: SerializeField] public bool isOnProcessorCycle { get; private set; }

        [SerializeField] private List<Processor> m_processors;
        [SerializeField] private List<Scheduler> m_schedulers;
        // TODO coolers

        public int index { get {
            for(int i=0; i<GameState.current.cores.Count; i++){
                if(GameState.current.cores[i] == this){
                    return i;
                }
            }
            return -1;
        } }

        public IReadOnlyList<Processor> processors => m_processors;
        public IReadOnlyList<Scheduler> schedulers => m_schedulers;
        
        public event System.Action<bool> onRunStateChanged = delegate {};
        public event System.Action onLayoutChanged = delegate {};
        public event System.Action onCycleChanged = delegate {};
        public event System.Action onUnlocked = delegate {};

        public Core () {
            unlocked = false;
            running = false;
            temperature = DEFAULT_TEMPERATURE;
            cycleTimer = 0;
            isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
            m_processors = new List<Processor>();
            m_schedulers = new List<Scheduler>();
        }

        // TODO
        public float temperatureSpeedFactor { get {
            return 1f;
        } }

        public float cycleDuration { get {
             return 1f / (temperatureSpeedFactor * TEMP_CYCLES_PER_SECOND);
        } }

        public IEnumerable<CoreComponent> components { get {
            foreach(var processor in processors){
                yield return processor;
            }
            foreach(var scheduler in schedulers){
                yield return scheduler;
            }
        } }

        public int remainingSlots { get {
            var output = SLOT_COUNT;
            foreach(var component in components){
                output -= component.slotSize;
            }
            return output;
        } }

        public int nextFreeSlotIndex { get {
            return SLOT_COUNT - remainingSlots;
        } }

        void ISerializationCallbackReceiver.OnBeforeSerialize () { }

        void ISerializationCallbackReceiver.OnAfterDeserialize () {
            foreach(var component in components){
                component.PostDeserializeInit(this);
            }
        }

        public void OnUpdate () {
            if(!(GameState.current.running || this.running)){
                return;
            }
            if(GameState.current.running && this.unlocked && !this.running){
                running = true;
                onRunStateChanged(running);
                ExecuteComponentsForCycle();
            }
            if(running){
                cycleTimer += (Time.deltaTime / cycleDuration);
                while(cycleTimer > 1){
                    if(!GameState.current.running){
                        cycleTimer = 0;
                        running = false;
                        onRunStateChanged(running);
                        isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
                        onCycleChanged();
                    }else{
                        cycleTimer -= 1;
                        isOnProcessorCycle = !isOnProcessorCycle;
                        onCycleChanged();
                        ExecuteComponentsForCycle();
                    }
                }
            }
        }

        void ExecuteComponentsForCycle () {
            if(isOnProcessorCycle){
                foreach(var processor in processors){
                    processor.Execute();
                }
            }else{
                foreach(var scheduler in schedulers){
                    scheduler.Execute();
                }
            }
        }

        public void OnFixedUpdate () {
            // TODO cooldown
        }

        public void Unlock () {
            if(!unlocked){
                unlocked = true;
                onUnlocked();
            }
        }

    }

}