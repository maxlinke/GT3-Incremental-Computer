using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cores.Components;

namespace Cores {

    [System.Serializable]
    public class Core : ISerializationCallbackReceiver {

        public const int SLOT_COUNT = 6;

        private const float MAX_TEMPERATURE = 115;
        private const float DEFAULT_COOLDOWN_IMPULSE_STRENGTH = 0.001f;

        public const bool START_ON_PROCESSOR_CYCLE = false;
        public const float TEMP_CYCLES_PER_SECOND = 2f;

        public struct TemperatureImpulse {
            public float targetTemperature;
            public float impulseStrength;

            public TemperatureImpulse WithTargetTemperature (float inputTargetTemperature) {
                return new TemperatureImpulse(){
                    targetTemperature = inputTargetTemperature,
                    impulseStrength = this.impulseStrength
                };
            }

            public TemperatureImpulse WithImpulseStrength (float inputImpulseStrength) {
                return new TemperatureImpulse(){
                    targetTemperature = this.targetTemperature,
                    impulseStrength = inputImpulseStrength
                };
            }
        }

        private static readonly TemperatureImpulse defaultCoolImpulse = new TemperatureImpulse(){
            targetTemperature = GameState.DEFAULT_TEMPERATURE,
            impulseStrength = DEFAULT_COOLDOWN_IMPULSE_STRENGTH
        };

        public static Core GetInitialCore (System.Func<ID> getNewId) {
            var output = new Core();
            output.unlocked = true;
            output.AddNewComponent<Processor>(getNewId(), 0);
            output.AddNewComponent<Scheduler>(getNewId(), 0);
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
        [SerializeField] private List<Cooler> m_coolers;

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
        public IReadOnlyList<Cooler> coolers => m_coolers;
        
        public event System.Action<bool> onRunStateChanged = delegate {};
        public event System.Action onLayoutChanged = delegate {};
        public event System.Action onCycleChanged = delegate {};
        public event System.Action onUnlocked = delegate {};

        public Core () {
            unlocked = false;
            running = false;
            temperature = GameState.DEFAULT_TEMPERATURE;
            cycleTimer = 0;
            isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
            m_processors = new List<Processor>();
            m_schedulers = new List<Scheduler>();
            m_coolers = new List<Cooler>();
        }

        public float temperatureSpeedFactor { get {
            if(temperature >= MAX_TEMPERATURE){
                return 0f;
            }
            return Mathf.Sqrt(MAX_TEMPERATURE - temperature) / Mathf.Sqrt(MAX_TEMPERATURE - GameState.DEFAULT_TEMPERATURE);
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
            foreach(var cooler in coolers){
                yield return cooler;
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
                component.SetCore(this);
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
            AddTemperatureImpulse(defaultCoolImpulse);
            foreach(var cooler in coolers){
                AddTemperatureImpulse(cooler.GetCoolImpulse());
            }
        }

        public void AddTemperatureImpulse (TemperatureImpulse impulse) {
            temperature = Mathf.Lerp(temperature, impulse.targetTemperature, impulse.impulseStrength);
        }

        public void Unlock () {
            if(!unlocked){
                unlocked = true;
                onUnlocked();
            }
        }

        public void AddNewProcessor (int level) {
            AddNewComponent<Processor>(ID.GetNext(), level);
        }

        public void AddNewScheduler (int level) {
            AddNewComponent<Scheduler>(ID.GetNext(), level);
        }

        public void AddNewCooler (int level) {
            AddNewComponent<Cooler>(ID.GetNext(), level);
        }

        void AddNewComponent<T> (ID id, int level) where T : CoreComponent, new() {
            var newT = new T();
            newT.id = id;
            newT.levelIndex = level;
            newT.upgradeCount = 0;
            AddComponent(newT);
        }

        public void AddComponent (CoreComponent newComponent) {
            newComponent.SetCore(this);
            newComponent.slotIndex = nextFreeSlotIndex;
            if(newComponent is Processor proc){
                m_processors.Add(proc);
            }else if(newComponent is Scheduler sched){
                m_schedulers.Add(sched);
            }else if(newComponent is Cooler cool){
                m_coolers.Add(cool);
            }
            onLayoutChanged();
        }

        public void RemoveComponent (CoreComponent removeComponent) {
            bool removed = false;
            if(removeComponent is Processor proc){
                removed = m_processors.Remove(proc);
            }else if(removeComponent is Scheduler sched){
                removed = m_schedulers.Remove(sched);
            }else if(removeComponent is Cooler cool){
                removed = m_coolers.Remove(cool);
            }
            if(removed){
                foreach(var component in components){
                    if(component.slotIndex > removeComponent.slotIndex){
                        component.slotIndex -= removeComponent.slotSize;
                    }
                }
                onLayoutChanged();
            }
        }

    }

}