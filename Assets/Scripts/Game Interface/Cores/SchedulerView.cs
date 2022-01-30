using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class SchedulerView : CoreComponentView<Scheduler> {

        [System.Serializable]
        public class ViewInitData {
            [field: SerializeField] public int blipImageRows;
            [field: SerializeField] public int blipImageCols;
        }

        [SerializeField, RedIfEmpty] Image m_blipImageTemplate;
        [SerializeField, RedIfEmpty] float m_horizontalBlipSpacing;
        [SerializeField, RedIfEmpty] float m_minBlipWidth;

        public Scheduler scheduler { get; private set; }

        public override CoreComponent component => scheduler;

        bool m_executing;
        bool m_updateImage;
        bool m_newImageState;
        Image[] m_blipImages;

        public override void Initialize (Scheduler scheduler) {
            base.Initialize(scheduler);
            this.scheduler = scheduler;
            scheduler.onExecute += OnExecute;
            InitImages();
            m_text.text = $"SCH {scheduler.id}";
        }

        void InitImages () {
            m_blipImageTemplate.SetGOActive(false);
            var initData = scheduler.level.viewInitData;
            var imageParentRT = m_blipImageTemplate.transform.parent as RectTransform;
            var newWidth = (m_blipImageTemplate.rectTransform.rect.width / initData.blipImageCols) - ((initData.blipImageCols - 1) * m_horizontalBlipSpacing);
            newWidth = Mathf.Max(newWidth, m_minBlipWidth);
            var offset = new Vector2(-newWidth - m_horizontalBlipSpacing, 0);
            m_blipImages = new Image[initData.blipImageRows * initData.blipImageCols];
            for(int i=0; i<m_blipImages.Length; i++){
                var ix = i % initData.blipImageCols;
                var iy = i / initData.blipImageCols;
                var newImage = Instantiate(m_blipImageTemplate, imageParentRT);
                m_blipImages[i] = newImage;
                newImage.rectTransform.SetAnchor(1, (iy + 0.5f) / initData.blipImageRows);
                newImage.rectTransform.SetAnchoredY(0);
                newImage.rectTransform.SetWidth(newWidth);
                newImage.rectTransform.anchoredPosition += ix * offset;
                newImage.SetGOActive(true);
            }
        }

        void OnExecute () {
            m_executing = true;
            m_updateImage = true;
            m_newImageState = scheduler.actuallyAddedTasks;
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
                foreach(var blipImage in m_blipImages){
                    blipImage.SetGOActive(m_newImageState);
                }
            }
        }

    }

}