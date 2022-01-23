using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class CoolerView : CoreComponentView<Cooler> {

        [SerializeField, RedIfEmpty] Image m_spinnerImage;
        [SerializeField] Sprite[] m_spinnerSprites;
        [SerializeField] float m_cycleDuration;

        public Cooler cooler { get; private set; }

        public override CoreComponent component => cooler;

        float m_timerOffset;
        int m_shownSpriteIndex;

        public override void Initialize (Cooler cooler) {
            base.Initialize(cooler);
            this.cooler = cooler;
            m_text.text = $"CLR {this.cooler.id}";
            m_timerOffset = Random.value;
            m_shownSpriteIndex = -1;
        }

        void Update () {
            var i = (int)(((Time.time / m_cycleDuration) + m_timerOffset) * m_spinnerSprites.Length) % m_spinnerSprites.Length;
            if(i != m_shownSpriteIndex){
                m_spinnerImage.sprite = m_spinnerSprites[i];
                m_shownSpriteIndex = i;
            }
        }

    }

}
