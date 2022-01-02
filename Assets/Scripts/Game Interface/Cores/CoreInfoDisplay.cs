using UnityEngine;
using UniRx;

namespace Cores {

    public class CoreInfoDisplay : MonoBehaviour {

        [SerializeField, RedIfEmpty] CoreInfo m_infoTemplate;
        [SerializeField, Min(0)] float m_updateInterval;

        CoreInfo[] m_infos;
        float m_nextUpdate;

        public void Initialize (CoreDisplay coreDisplay) {
            SpawnDisplays(coreDisplay);
            GameState.onRunStateChanged.Subscribe(_ => UpdateDisplay());
            foreach(var core in coreDisplay.Cores){
                core.onUnlockStateChanged.Subscribe(_ => UpdateDisplay());
            }
            UpdateDisplay();
            m_nextUpdate = -1;
        }

        void Update () {
            if(Time.time > m_nextUpdate){
                UpdateDisplay();
                m_nextUpdate = Time.time + m_updateInterval;
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