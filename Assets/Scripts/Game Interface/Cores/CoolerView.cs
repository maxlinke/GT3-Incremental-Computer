using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Cores.Components {

    public class CoolerView : CoreComponentView<Cooler> {

        class ImageData {
            public IReadOnlyList<Sprite> sprites;
            public float timerOffset;
            public int shownSpriteIndex;
        }

        [SerializeField, RedIfEmpty] Image m_imageTemplate;
        [SerializeField, RedIfEmpty] CoolerSprites m_sprites;
        [SerializeField] float m_cycleDuration;
        [SerializeField] float m_horizontalImageSpacing;

        public Cooler cooler { get; private set; }

        public override CoreComponent component => cooler;

        Dictionary<Image, ImageData> m_images;

        public override void Initialize (Cooler cooler) {
            base.Initialize(cooler);
            this.cooler = cooler;
            m_text.text = $"CLR {this.cooler.id}";
            InitImages();
        }

        void InitImages () {
            m_images = new Dictionary<Image, ImageData>();
            m_imageTemplate.SetGOActive(false);
            var level = Cooler.Level.levels[cooler.levelIndex];
            var imageParentRT = m_imageTemplate.transform.parent as RectTransform;
            var offset = (-1) * new Vector2(m_imageTemplate.rectTransform.rect.width + m_horizontalImageSpacing, imageParentRT.rect.height / level.slotSize);
            for(int i=0; i<level.slotSize; i++){
                for(int j=0; j<level.spriteColumnCount; j++){
                    var newImage = Instantiate(m_imageTemplate, imageParentRT);
                    newImage.SetGOActive(true);
                    newImage.rectTransform.anchoredPosition += offset * new Vector2(j, i);
                    m_images[newImage] = new ImageData(){
                        sprites = GetSprites(level, i, j),
                        timerOffset = Random.value,
                        shownSpriteIndex = -1
                    };
                }
            }
        }

        IReadOnlyList<Sprite> GetSprites (Cooler.Level level, int i, int j) {
            if(level.useFanSprites && level.useIceSprites){
                return (((i + j) % 2) == 0) ? m_sprites.fanSprites : m_sprites.iceSprites;
            }else if(level.useFanSprites){
                return m_sprites.fanSprites;
            }else if(level.useIceSprites){
                return m_sprites.iceSprites;
            }
            Debug.LogWarning($"No sprites set for cooler level {cooler.levelIndex}!");
            return m_sprites.fanSprites;
        }

        void Update () {
            foreach(var image in m_images.Keys){
                var data = m_images[image];
                var i = (int)(((Time.time / m_cycleDuration) + data.timerOffset) * data.sprites.Count) % data.sprites.Count;
                if(i != data.shownSpriteIndex){
                    image.sprite = data.sprites[i];
                    data.shownSpriteIndex = i;
                }
            }
        }

    }

}
