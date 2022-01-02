using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

namespace Cores {

    public class Core : MonoBehaviour {

        [System.Serializable]
        public class State {

            [SerializeField] public bool unlocked;
            [SerializeField] public float temperature;
            [SerializeField] public float cycleTimer;
            [SerializeField] public bool isOnProcessorCycle;

            public State () {
                unlocked = false;
                temperature = DEFAULT_TEMPERATURE;
                cycleTimer = 0;
                isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
            }

            public static State GetNewStateForInitialCore () {
                var output = new State();
                output.unlocked = true;
                // TODO basic processor
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
        [SerializeField, RedIfEmpty] Processor m_processorTemplate;

        public int index { get; private set; }
        public State state { get; private set; }

        public bool isRunning => isUnlocked && GameState.current.running;
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


        Slot[] m_slots;

        public Subject<bool> onUnlockStateChanged { get; private set; } = new Subject<bool>();

        public RectTransform rectTransform => (RectTransform)transform;

        public void Initialize (int index) {
            this.index = index;
            this.name = $"Core {index}";
            m_headerText.text = this.name;
            SpawnSlots("Empty slot");
            GameState.onGameStateChanged.Subscribe(OnGameStateChanged);
            GameState.onRunStateChanged.Subscribe(OnRunStateChanged);
            OnGameStateChanged(GameState.current);
        }

        void OnGameStateChanged (GameState gameState) {
            this.state = gameState.coreStates[this.index];
            OnRunStateChanged(gameState.running);
            onUnlockStateChanged.OnNext(isUnlocked);
            // components...
            // TEMP
            m_processorTemplate.SetGOActive(false);
        }

        void OnRunStateChanged (bool running) {
            UpdateElementColors();
            state.cycleTimer = 0;
            state.isOnProcessorCycle = START_ON_PROCESSOR_CYCLE;
        }

        void Update () {
            if(isRunning){
                AdvanceClockAndExecute();
            }
            Cooldown();
        }

        void AdvanceClockAndExecute () {
            var timer = state.cycleTimer + (Time.deltaTime * temperatureSpeedFactor) * TEMP_CYCLES_PER_SECOND;
            var cycles = (int)timer;
            for(int i=0; i<cycles; i++){
                var processorState = !state.isOnProcessorCycle;
                if(processorState){
                    // trigger processors
                }else{
                    // trigger schedulers
                }
                m_processorCycleImage.color = processorState ? onColor : offColor;
                m_schedulerCycleImage.color = processorState ? offColor : onColor;
                state.isOnProcessorCycle = processorState;
            }
            state.cycleTimer = timer - cycles;
        }

        void Cooldown () {

        }

        void UpdateElementColors () {
            var color = GetUnlockStateColor(this.isUnlocked);
            m_frame.color = color;
            m_divider.color = color;
            m_headerText.color = color;
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
                newSlot.isFree = true;
            }
        }

    }

}