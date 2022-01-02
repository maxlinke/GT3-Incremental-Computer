using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class VisualScheduler : VisualCoreComponent<Cores.Components.Scheduler> {

        [SerializeField, RedIfEmpty] Image m_blipImage;

        public Scheduler scheduler { get; private set; }

        public override int slotSize => scheduler.slotSize;

        bool m_executing;
        float m_offTime;

        public override void Initialize(Scheduler inputScheduler) {
            this.scheduler = inputScheduler;
            m_blipImage.SetGOActive(false);
            m_text.text = $"SCH {scheduler.id}";
        }

        public override void Execute(float expectedDuration) {
            scheduler.Execute();
            m_executing = true;
            m_blipImage.SetGOActive(true);
            m_offTime = Time.time + 0.5f * expectedDuration;
        }

        void Update () {
            if(m_executing){
                if(Time.time > m_offTime){
                    m_blipImage.SetGOActive(false);
                    m_executing = false;
                }
            }
        }

    }

}