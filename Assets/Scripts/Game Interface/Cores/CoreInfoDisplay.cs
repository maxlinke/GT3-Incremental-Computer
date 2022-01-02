using UnityEngine;

namespace Cores {

    public class CoreInfoDisplay : MonoBehaviour {

        [SerializeField, RedIfEmpty] CoreInfo m_infoTemplate;

        public void Initialize (CoreDisplay coreDisplay) {
            SpawnDisplays(coreDisplay);
        }

        void SpawnDisplays (CoreDisplay coreDisplay) {
            m_infoTemplate.SetGOActive(false);
            CoreDisplay.SpawnThingsAndAlignLikeCores((index) => {
                var newInfo = Instantiate(m_infoTemplate, m_infoTemplate.rectTransform.parent);
                newInfo.SetGOActive(true);
                newInfo.Initialize(coreDisplay.GetCore(index));
                return newInfo.rectTransform;
            });
        }

    }

}