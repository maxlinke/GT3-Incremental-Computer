using UnityEngine;
using UniRx;

namespace Cores {

    public class CoreInfoDisplay : MonoBehaviour {

        // TODO this can be simplified and simply update every frame... 

        [SerializeField, RedIfEmpty] CoreInfo m_infoTemplate;
        [SerializeField, Min(0)] float m_updateInterval;

        CoreInfo[] m_infos;
        float m_nextUpdate;
        bool m_updateScheduled;

        public void Initialize (CoreDisplay coreDisplay) {
            SpawnDisplays(coreDisplay);
            GameState.onGameStateChanged.Subscribe(_ => ScheduleUpdate());
            GameState.onRunStateChanged.Subscribe(_ => ScheduleUpdate());
            foreach(var core in coreDisplay.Cores){
                core.onUnlockStateChanged.Subscribe(_ => ScheduleUpdate());
            }
            UpdateDisplay();
            m_nextUpdate = -1;
        }

        void ScheduleUpdate () {
            m_updateScheduled = true;
        }

        void Update () {
            if(Time.time > m_nextUpdate || m_updateScheduled){
                UpdateDisplay();
                m_nextUpdate = Time.time + m_updateInterval;
                m_updateScheduled = false;
            }
        }

        void SpawnDisplays (CoreDisplay coreDisplay) {
            m_infoTemplate.SetGOActive(false);
            m_infos = new CoreInfo[CoreDisplay.NUMBER_OF_CORES];
            CoreDisplay.SpawnThingsAndAlignLikeCores((index) => {
                var newInfo = Instantiate(m_infoTemplate, m_infoTemplate.rectTransform.parent);
                newInfo.SetGOActive(true);
                newInfo.Initialize(coreDisplay.GetCore(index));
                m_infos[index] = newInfo;
                return newInfo.rectTransform;
            });
        }

        void UpdateDisplay () {
            foreach(var info in m_infos){
                info.UpdateInfo();
            }
        }

    }

}