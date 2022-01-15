using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class ProcessorView : CoreComponentView<Processor> {

        [SerializeField, RedIfEmpty] Image m_usageImage;

        public Processor processor { get; private set; }

        public override CoreComponent component => processor;

        bool m_updateImage;
        bool m_newImageState;
        float m_usageLevel;
        bool m_executing;

        public override void Initialize (Processor processor) {
            base.Initialize(processor);
            this.processor = processor;
            processor.onExecute += OnExecute;
            m_usageImage.SetGOActive(false);
            m_text.text = $"PRC {this.processor.id}";
        }

        void OnExecute (float usageLevel) {
            m_updateImage = true;
            m_executing = true;
            m_newImageState = true;
            m_usageLevel = usageLevel;
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
                m_usageImage.SetGOActive(m_newImageState);
                if(m_newImageState){
                    m_usageImage.fillAmount = m_usageLevel;
                }
            }
        }

    }

}