using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class SchedulerView : CoreComponentView<Scheduler> {

        [SerializeField, RedIfEmpty] Image m_blipImage;

        public Scheduler scheduler { get; private set; }

        public override CoreComponent component => scheduler;

        bool m_executing;
        bool m_updateImage;
        bool m_newImageState;

        public override void Initialize (Scheduler scheduler) {
            this.scheduler = scheduler;
            scheduler.onExecute += OnExecute;
            m_blipImage.SetGOActive(false);
            m_text.text = $"SCH {scheduler.id}";
        }

        void OnExecute () {
            m_executing = true;
            m_updateImage = true;
            m_newImageState = true;
        }

        void LateUpdate () {
            if(m_executing){
                if(!scheduler.core.running || scheduler.core.cycleTimer > 0.5f){
                    m_executing = false;
                    m_updateImage = true;
                    m_newImageState = false;
                }
            }
            if(m_updateImage){
                m_blipImage.SetGOActive(m_newImageState);
            }
        }

    }

}