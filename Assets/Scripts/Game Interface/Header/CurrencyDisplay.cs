using UnityEngine;
using UnityEngine.UI;

namespace GameInterfaceElements.Header {

    public class CurrencyDisplay : MonoBehaviour {

        [SerializeField, RedIfEmpty] Text m_mainText;
        [SerializeField, RedIfEmpty] Text m_gainText;

        bool m_updateText;
        int m_displayValue;

        float m_nextGainUpdate;
        int m_pointsAtLastGainUpdate;

        System.Text.StringBuilder m_niceNumberStringBuilder;

        public void Initialize () {
            GameState.onGameStateChanged += OnGameStateChanged;
            GameState.onCurrencyChanged += OnCurrencyChanged;
            OnGameStateChanged(GameState.current);
            OnCurrencyChanged(GameState.current.currency);
            m_niceNumberStringBuilder = new System.Text.StringBuilder();
        }

        void OnGameStateChanged (GameState gameState) {
            m_pointsAtLastGainUpdate = GameState.current.currency;
            m_nextGainUpdate = Time.time;
        }

        void OnCurrencyChanged (int newCurrency) {
            m_displayValue = newCurrency;
            m_updateText = true;
        }

        void LateUpdate () {
            if(Time.time >= m_nextGainUpdate){
                var delta = m_displayValue - m_pointsAtLastGainUpdate;
                m_gainText.text = $"{NumberWithPeriods(delta)} {GameState.CURRENCY_SYMBOL}/s";
                m_nextGainUpdate = Time.time + 1f;
                m_pointsAtLastGainUpdate = m_displayValue;
                m_updateText = true;
            }
            if(m_updateText){
                m_mainText.text = $"{NumberWithPeriods(m_displayValue)} {GameState.CURRENCY_SYMBOL}";
                m_updateText = false;
            }
        }

        string NumberWithPeriods (int inputNumber) {
            m_niceNumberStringBuilder.Clear();
            var rawString = inputNumber.ToString();
            for(int i=0; i<rawString.Length; i++){
                var c = rawString[rawString.Length - 1 - i];
                if((i > 0) && ((i % 3) == 0) && (char.IsNumber(c))){
                    m_niceNumberStringBuilder.Insert(0, '.');
                }
                m_niceNumberStringBuilder.Insert(0, c);
            }
            return m_niceNumberStringBuilder.ToString();
        }

    }

}