using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cores.Components;
using Scheduler = Cores.Components.Scheduler;

namespace Cores {

    public class Core : MonoBehaviour {

        // TODO heat
        // TODO proper values
        // TODO proper adding and removing (giving back ids) of components
        // TODO maybe start/end execution cycle -> image on the entire time?

        [System.Serializable]
        public class State {

            [SerializeField] public bool unlocked;
            [SerializeField] public float temperature;
            [SerializeField] public float cycleTimer;
            [SerializeField] public bool isOnProcessorCycle;
            [SerializeField] public List<Processor> processors;
            [SerializeField] public List<Scheduler> schedulers;
            // TODO coolers

            public State () {
                unlocked = false;
                temperature = DEFAULT_TEMPERATURE;
                cycleTimer = 0;
                isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
                processors = new List<Processor>();
                schedulers = new List<Scheduler>();
            }

            public static State GetNewStateForInitialCore (System.Func<ID> getNewId) {
                var output = new State();
                output.unlocked = true;
                output.processors.Add(new Processor(getNewId()));
                output.schedulers.Add(new Scheduler(getNewId()));
                // TODO basic scheduler
                return output;
            }

        }

        public const int SLOT_COUNT = 6;
        public const float INACTIVE_ALPHA = 0.4f;
        public const float DEFAULT_TEMPERATURE = 21;
        public const bool START_ON_PROCESSOR_CYCLE = false;

        public const float TEMP_CYCLES_PER_SECOND = 2f;

        public static Color GetUnlockStateColor (bool unlocked) => unlocked ? onColor : offColor;

        public static readonly Color onColor = new Color(1f, 1f, 1f, 1f);
        public static readonly Color offColor = new Color(1f, 1f, 1f, 0.4f);

        [Header("Basic Components")]
        [SerializeField, RedIfEmpty] Image m_frame;
        [SerializeField, RedIfEmpty] Text m_headerText;
        [SerializeField, RedIfEmpty] Image m_divider;
        [SerializeField, RedIfEmpty] Image m_processorCycleImage;
        [SerializeField, RedIfEmpty] Image m_schedulerCycleImage;
        [SerializeField, RedIfEmpty] RectTransform m_slotArea;

        [Header("The other components")]
        [SerializeField, RedIfEmpty] Slot m_slotTemplate;
        [SerializeField, RedIfEmpty] VisualProcessor m_processorTemplate;
        [SerializeField, RedIfEmpty] VisualScheduler m_schedulerTemplate;

        public int index { get; private set; }
        public State state { get; private set; }

        public bool isRunning => (isUnlocked && GameState.current.running) || (state.cycleTimer > 0);
        public float temperature => state.temperature;
        public float temperatureSpeedFactor { get {
            if(!isRunning){
                return 0f;
            }
            // TODO
            return 1f;
        } }

        public bool isUnlocked { 
            get {
                return state.unlocked;
            } set {
                if(state.unlocked != value){
                    state.unlocked = value;
                    onUnlockStateChanged.OnNext(value);
                    UpdateElementColors();
                }
            }
        }

        public int remainingSlots { get {
            var output = SLOT_COUNT;
            foreach(var comp in m_comps){
                output -= comp.slotSize;
            }
            return output;
        } }

        Slot[] m_slots;
        List<CoreComponent> m_comps;
        List<VisualCoreComponent> m_visComps;

        bool m_redoInternalLayout;

        public Subject<bool> onUnlockStateChanged { get; private set; } = new Subject<bool>();

        public RectTransform rectTransform => (RectTransform)transform;

        public void Initialize (int index) {
            this.index = index;
            this.name = $"Core {index}";
            m_headerText.text = this.name;
            SpawnSlots("Empty slot");
            InitVisualComponents();
            GameState.onGameStateChanged.Subscribe(OnGameStateChanged);
            GameState.onRunStateChanged.Subscribe(OnRunStateChanged);
            OnGameStateChanged(GameState.current);
        }

        void OnGameStateChanged (GameState gameState) {
            this.state = gameState.coreStates[this.index];
            OnRunStateChanged(gameState.running);
            onUnlockStateChanged.OnNext(isUnlocked);
            RespawnVisualComponents();
        }

        void OnRunStateChanged (bool running) {
            UpdateElementColors();
        }

        void Update () {
            if(isRunning){
                AdvanceClockAndExecute();
            }
        }

        void LateUpdate () {
            if(m_redoInternalLayout){
                RedoInternalLayout();
                m_redoInternalLayout = false;
            }
        }

        void FixedUpdate () {
            Cooldown();
        }

        void AdvanceClockAndExecute () {
            var cycleDuration = 1f / (temperatureSpeedFactor * TEMP_CYCLES_PER_SECOND);
            state.cycleTimer += (Time.deltaTime / cycleDuration);
            if(state.cycleTimer > 1){
                if(!GameState.current.running){
                    state.cycleTimer = 0;
                    state.isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
                }else{
                    state.cycleTimer -= (int)(state.cycleTimer);
                    state.isOnProcessorCycle = !state.isOnProcessorCycle;
                    if(state.isOnProcessorCycle){
                        foreach(var comp in m_visComps){
                            if(comp is VisualProcessor proc){
                                proc.Execute(cycleDuration);
                            }
                        }
                    }else{
                        foreach(var comp in m_visComps){
                            if(comp is VisualScheduler sched){
                                sched.Execute(cycleDuration);
                            }
                        }
                    }
                }
                UpdateCycleImages();
            }
        }

        void Cooldown () {

        }

        void RespawnVisualComponents () {
            foreach(var comp in m_visComps){
                Destroy(comp.gameObject);
            }
            m_visComps.Clear();
            foreach(var proc in state.processors){
                AddComponent(proc);
            }
            foreach(var sched in state.schedulers){
                AddComponent(sched);
            }
        }

        void AddComponent (CoreComponent component) {
            if(component is Processor proc){
                var newProc = Instantiate(m_processorTemplate, m_slotArea);
                newProc.SetGOActive(true);
                newProc.Initialize(proc);
                m_visComps.Add(newProc);
            }else if(component is Scheduler sched){
                var newSched = Instantiate(m_schedulerTemplate, m_slotArea);
                newSched.SetGOActive(true);
                newSched.Initialize(sched);
                m_visComps.Add(newSched);
            }else{
                // ...
            }
            m_redoInternalLayout = true;
        }

        void UpdateElementColors () {
            var color = GetUnlockStateColor(this.isUnlocked);
            m_frame.color = color;
            m_divider.color = color;
            m_headerText.color = color;
            UpdateCycleImages();
        }

        void UpdateCycleImages () {
            if(isRunning){
                m_processorCycleImage.color = (state.isOnProcessorCycle) ? onColor : offColor;
                m_schedulerCycleImage.color = (state.isOnProcessorCycle) ? offColor : onColor;
            }else{
                m_processorCycleImage.color = offColor;
                m_schedulerCycleImage.color = offColor;
            }
        }

        void SpawnSlots (string slotText) {
            m_slotTemplate.SetGOActive(false);
            m_slots = new Slot[SLOT_COUNT];
            var y = 0f;
            for(int i=0; i<SLOT_COUNT; i++){
                var newSlot = Instantiate(m_slotTemplate, m_slotArea);
                newSlot.SetGOActive(true);
                newSlot.rectTransform.SetAnchoredY(y);
                y -= newSlot.rectTransform.rect.height;
                m_slots[i] = newSlot;
                newSlot.color = offColor;
                newSlot.text = slotText;
            }
        }

        void InitVisualComponents () {
            m_processorTemplate.SetGOActive(false);
            m_schedulerTemplate.SetGOActive(false);
            m_visComps = new List<VisualCoreComponent>();
        }

        void RedoInternalLayout () {
            var slotIndex = 0;
            foreach(var comp in m_visComps){
                comp.rectTransform.SetAnchoredY(m_slots[slotIndex].rectTransform.anchoredPosition.y);
                for(int i=0; i<comp.slotSize; i++){
                    m_slots[slotIndex].SetGOActive(false);
                    slotIndex++;
                }
            }
            for(int i=slotIndex; i<SLOT_COUNT; i++){
                m_slots[i].SetGOActive(true);
            }
        }

    }

}