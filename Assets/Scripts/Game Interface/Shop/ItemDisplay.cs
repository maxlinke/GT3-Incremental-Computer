using UnityEngine;
using UnityEngine.UI;

namespace Shops {

    public class ItemDisplay : MonoBehaviour {

        [SerializeField, RedIfEmpty] CanvasGroup m_canvasGroup;
        [SerializeField, RedIfEmpty] Text m_itemNameText;
        [SerializeField, RedIfEmpty] Text m_priceText;
        [SerializeField, RedIfEmpty] Text m_infoText;

        public RectTransform rectTranform => (RectTransform)transform;

        public Item item { get; private set; }

        bool m_initialized = false;
        bool m_subscribed = false;
        bool m_updateDisplay;

        public void Initialize (Item item) {
            gameObject.SetActive(true);
            this.item = item;
            SetTextAndUpdateHeight(m_itemNameText, item.displayName, out var nameHeight);
            var priceString = $"{GameInterfaceElements.Header.CurrencyDisplay.NumberWithPeriods(item.price)} {GameState.CURRENCY_SYMBOL}";
            SetTextAndUpdateHeight(m_priceText, priceString, out var priceHeight);
            SetTextAndUpdateHeight(m_infoText, item.info, out var infoHeight);
            rectTranform.SetHeight(Mathf.Max(nameHeight, priceHeight, infoHeight));
            GameState.onGameStateChanged += OnGameStateChanged;
            OnGameStateChanged(GameState.current);
            m_initialized = true;
            SetCurrencyUpdateSubscription(true);

            void SetTextAndUpdateHeight (Text text, string newText, out float newHeight) {
                text.text = newText;
                newHeight = text.preferredHeight;
                text.rectTransform.SetHeight(newHeight);
            }
        }

        void OnGameStateChanged (GameState gameState) => m_updateDisplay = true;
        void OnCurrencyChanged (int newValue) => m_updateDisplay = true;

        void OnEnable () {
            if(m_initialized && !m_subscribed){
                SetCurrencyUpdateSubscription(true);
            }
            m_updateDisplay = true;
        }

        void OnDisable () {
            if(m_subscribed){
                SetCurrencyUpdateSubscription(false);
            }
        }

        void SetCurrencyUpdateSubscription (bool value) {
            if(value != m_subscribed){
                if(value){
                    GameState.onCurrencyChanged += OnCurrencyChanged;
                }else{
                    GameState.onCurrencyChanged -= OnCurrencyChanged;
                }
                m_subscribed = value;
            }
        }

        void LateUpdate () {
            if(m_updateDisplay){
                m_canvasGroup.alpha = item.CurrentlyPurchaseable(out _) ? 1 : GameInterface.INACTIVE_COLOR_FACTOR;
                m_updateDisplay = false;
            }
        }

    }

}