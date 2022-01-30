using System.Collections.Generic;
using UnityEngine;

namespace Cores.Components {

    public class ProcessorView : CoreComponentView<Processor> {

        struct LevelGaugeSetup {
            public float widthMultiplier;
            public int columnCount;
        }

        static readonly LevelGaugeSetup[] gaugeSetups = new LevelGaugeSetup[]{
            new LevelGaugeSetup(){
                widthMultiplier = 1,
                columnCount = 1
            },new LevelGaugeSetup(){
                widthMultiplier = 1.1f,
                columnCount = 2
            },new LevelGaugeSetup(){
                widthMultiplier = 1.25f,
                columnCount = 3
            },
        };

        [SerializeField, RedIfEmpty] UsageGauge m_usageGaugeTemplate;

        public Processor processor { get; private set; }

        public override CoreComponent component => processor;

        UsageGauge[] m_gauges;
        bool m_updateImage;
        bool m_newImageState;
        float m_usageLevel;
        bool m_executing;

        public override void Initialize (Processor processor) {
            base.Initialize(processor);
            this.processor = processor;
            processor.onExecute += OnExecute;
            processor.onUpgraded += OnUpgraded;
            InitGauges();
            OnUpgraded(processor.upgradeCount);
        }

        void InitGauges () {
            m_usageGaugeTemplate.SetGOActive(false);
            var setupIndex = processor.levelIndex;
            if(setupIndex >= gaugeSetups.Length){
                Debug.LogWarning($"Graphics not set up for processor level \"{setupIndex}\"!");
                setupIndex = 0;
            }
            var setup = gaugeSetups[setupIndex];
            var rowCount = processor.slotSize;
            var gaugeWidth = Mathf.RoundToInt(m_usageGaugeTemplate.rectTransform.rect.width * setup.widthMultiplier);
            var gaugeHeight = Mathf.RoundToInt(m_usageGaugeTemplate.rectTransformParent.rect.height / rowCount);
            var posOffset = (-1f) * new Vector2(gaugeWidth, gaugeHeight);
            var gauges = new List<UsageGauge>();
            for(int i=0; i<setup.columnCount; i++){
                for(int j=0; j<rowCount; j++){
                    if(j == 0  && i > 1) continue;
                    var newGauge = Instantiate(m_usageGaugeTemplate, m_usageGaugeTemplate.transform.parent);
                    gauges.Add(newGauge);
                    newGauge.name = $"Gauge ({i}, {j})";
                    newGauge.SetGOActive(true);
                    newGauge.ShowValue(0);
                    newGauge.rectTransform.SetWidth(gaugeWidth);
                    newGauge.rectTransform.SetHeight(gaugeHeight);
                    newGauge.rectTransform.anchoredPosition += Vector2.Scale(posOffset, new Vector2(i, j));
                }
            }
            m_gauges = gauges.ToArray();
        }

        void OnExecute (float usageLevel) {
            m_updateImage = true;
            m_executing = true;
            m_newImageState = true;
            m_usageLevel = usageLevel;
        }

        void OnUpgraded (int upgradeLevel) {
            m_text.text = $"PRC {this.processor.id} {GameState.GetUpgradeIndicator(upgradeLevel)}";
        }

        void LateUpdate () {
            if(m_executing){
                if(!processor.core.running || processor.core.cycleTimer > 0.5f){
                    m_executing = false;
                    m_updateImage = true;
                    m_newImageState = false;
                }
            }
            if(m_updateImage){
                var remaining = (m_newImageState ? m_usageLevel * m_gauges.Length : 0);
                for(int i=0; i<m_gauges.Length; i++){
                    m_gauges[i].ShowValue(Mathf.Clamp01(remaining));
                    remaining -= 1f;
                }
            }
        }

    }

}