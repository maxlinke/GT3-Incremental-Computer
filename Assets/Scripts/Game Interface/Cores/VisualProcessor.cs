using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class VisualProcessor : VisualCoreComponent<Cores.Components.Processor> {

        [SerializeField, RedIfEmpty] Image m_usageImage;

        public Cores.Components.Processor processor { get; private set; }

        public override int slotSize => processor.slotSize;

        bool m_executing;
        float m_offTime;

        public override void Initialize(Cores.Components.Processor value) {
            this.processor = value;
            m_usageImage.SetGOActive(false);
            m_text.text = $"PRC {processor.id}";
        }

        public override void Execute(float expectedDuration) {
            processor.Execute(out var usageLevel);
            m_usageImage.SetGOActive(true);
            m_usageImage.fillAmount = usageLevel;
            m_executing = true;
            m_offTime = Time.time + 0.5f * expectedDuration;
        }

        void Update () {
            if(m_executing){
                if(Time.time > m_offTime){
                    m_usageImage.SetGOActive(false);
                    m_executing = false;
                }
            }
        }

    }

}