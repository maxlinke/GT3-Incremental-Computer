using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cores.Components;
using Scheduler = Cores.Components.Scheduler;
using System.Linq;

namespace Cores {

    public class CoreView : MonoBehaviour {

        public static Color GetUnlockStateColor (bool unlocked) => unlocked ? onColor : offColor;

        public static readonly Color onColor = new Color(1f, 1f, 1f, 1f);
        public static readonly Color offColor = new Color(1f, 1f, 1f, GameInterface.INACTIVE_COLOR_FACTOR);

        [Header("Basic Components")]
        [SerializeField, RedIfEmpty] Image m_frame;
        [SerializeField, RedIfEmpty] Text m_headerText;
        [SerializeField, RedIfEmpty] Image m_divider;
        [SerializeField, RedIfEmpty] Image m_processorCycleImage;
        [SerializeField, RedIfEmpty] Image m_schedulerCycleImage;
        [SerializeField, RedIfEmpty] RectTransform m_slotArea;

        [Header("The other components")]
        [SerializeField, RedIfEmpty] Slot m_slotTemplate;
        [SerializeField, RedIfEmpty] ProcessorView m_processorTemplate;
        [SerializeField, RedIfEmpty] SchedulerView m_schedulerTemplate;
        [SerializeField, RedIfEmpty] CoolerView m_coolerTemplate;

        public int index { get; private set; }
        public Core core { get; private set; }

        Slot[] m_slots;
        List<CoreComponent> m_comps;
        List<CoreComponentView> m_compViews;

        bool m_redoInternalLayout;

        public event System.Action<Core> onCoreChanged = delegate {};
        public event System.Action<bool> onUnlockStateChanged = delegate {};

        public RectTransform rectTransform => (RectTransform)transform;

        public void Initialize (int index) {
            this.index = index;
            this.name = $"Core {index}";
            m_headerText.text = this.name;
            SpawnSlots("Empty slot");
            InitVisualComponents();
            GameState.onGameStateChanged += OnGameStateChanged;
            OnGameStateChanged(GameState.current);
        }

        void OnGameStateChanged (GameState gameState) {
            UnsubscribeCoreEvents();
            this.core = gameState.cores[this.index];
            SubscribeCoreEvents();
            OnRunStateChanged(gameState.running);
            onCoreChanged(this.core);
            onUnlockStateChanged(this.core.unlocked);
            RespawnVisualComponents();
            UpdateElementColors();
        }

        void SubscribeCoreEvents () {
            if(core != null){
                core.onRunStateChanged += OnRunStateChanged;
                core.onLayoutChanged += OnCoreLayoutChanged;
                core.onCycleChanged += UpdateElementColors;
                core.onUnlocked += OnCoreUnlocked;
            }
        }

        void UnsubscribeCoreEvents () {
            if(core != null){
                core.onRunStateChanged -= OnRunStateChanged;
                core.onLayoutChanged -= OnCoreLayoutChanged;
                core.onCycleChanged -= UpdateElementColors;
                core.onUnlocked -= OnCoreUnlocked;
            }
        }

        void OnRunStateChanged (bool running) {
            UpdateElementColors();
        }

        void OnCoreLayoutChanged () {
            m_redoInternalLayout = true;
        }

        void OnCoreUnlocked () {
            UpdateElementColors();
            onUnlockStateChanged(core.unlocked);
        }

        void Update () => core.OnUpdate();
        
        void FixedUpdate () => core.OnFixedUpdate();

        void LateUpdate () {
            if(m_redoInternalLayout){
                RedoInternalLayout();
                m_redoInternalLayout = false;
            }
        }

        void RespawnVisualComponents () {
            foreach(var comp in m_compViews){
                Destroy(comp.gameObject);
            }
            m_compViews.Clear();
            foreach(var proc in core.processors){
                AddComponent(proc);
            }
            foreach(var sched in core.schedulers){
                AddComponent(sched);
            }
            foreach(var cool in core.coolers){
                AddComponent(cool);
            }
            m_redoInternalLayout = true;
        }

        void AddComponent (CoreComponent component) {
            if(component is Processor proc){
                CreateView(proc, m_processorTemplate);
            }else if(component is Scheduler sched){
                CreateView(sched, m_schedulerTemplate);
            }else if(component is Cooler cool){
                CreateView(cool, m_coolerTemplate);
            }else{
                Debug.LogError($"Was asked to add {component.GetType().Name} but that's not handled...");
            }
            m_redoInternalLayout = true;

            void CreateView<T, U> (T comp, U template)
                where T : CoreComponent
                where U : CoreComponentView<T>
            {
                var newView = Instantiate(template, m_slotArea);
                newView.SetGOActive(true);
                newView.Initialize(comp);
                m_compViews.Add(newView);
            }
        }

        void RemoveView (CoreComponentView viewToRemove) {
            m_compViews.Remove(viewToRemove);
            Destroy(viewToRemove.gameObject);
        }

        void UpdateElementColors () {
            var color = GetUnlockStateColor(core.unlocked);
            m_frame.color = color;
            m_divider.color = color;
            m_headerText.color = color;
            UpdateCycleImages();
        }

        void UpdateCycleImages () {
            if(core != null && core.running){
                m_processorCycleImage.color = (core.isOnProcessorCycle) ? onColor : offColor;
                m_schedulerCycleImage.color = (core.isOnProcessorCycle) ? offColor : onColor;
            }else{
                m_processorCycleImage.color = offColor;
                m_schedulerCycleImage.color = offColor;
            }
        }

        void SpawnSlots (string slotText) {
            m_slotTemplate.SetGOActive(false);
            m_slots = new Slot[Core.SLOT_COUNT];
            var y = 0f;
            for(int i=0; i<m_slots.Length; i++){
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
            m_coolerTemplate.SetGOActive(false);
            m_compViews = new List<CoreComponentView>();
        }

        void RedoInternalLayout () {
            for(int i=0; i<m_slots.Length; i++){
                m_slots[i].SetGOActive(true);
            }
            var removeViews = new List<CoreComponentView>(m_compViews.Where(view => !core.components.Contains(view.component)));
            var newComps = new List<CoreComponent>(core.components.Where(component => m_compViews.Find(view => view.component == component) == null));
            foreach(var compView in removeViews){
                RemoveView(compView);
            }
            foreach(var newComp in newComps){
                AddComponent(newComp);
            }
            foreach(var compView in m_compViews){
                var slotIndex = compView.slotIndex;
                compView.rectTransform.SetAnchoredY(m_slots[slotIndex].rectTransform.anchoredPosition.y);
                for(int i=0; i<compView.slotSize; i++){
                    m_slots[slotIndex].SetGOActive(false);
                    slotIndex++;
                }
            }
        }

    }

}