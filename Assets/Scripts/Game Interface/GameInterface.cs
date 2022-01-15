using UnityEngine;
using GameInterfaceElements.Header;

public class GameInterface : MonoBehaviour {

    public const float INACTIVE_COLOR_FACTOR = 0.4f;

    [Header("Major Elements")]
    [SerializeField, RedIfEmpty] InputConsole m_console;
    [SerializeField, RedIfEmpty] TaskQueue m_queue;
    [SerializeField, RedIfEmpty] MainDisplay m_mainDisplay;

    [Header("Minor Elements")]
    [SerializeField, RedIfEmpty] StatusDisplay m_statusDisplay;
    [SerializeField, RedIfEmpty] CurrencyDisplay m_currencyDisplay;
    [SerializeField, RedIfEmpty] SystemClock m_systemClock;

    public void Initialize () {
        m_console.Initialize();
        m_mainDisplay.Initialize();
        m_queue.Initialize();

        m_statusDisplay.Initialize();
        m_currencyDisplay.Initialize();
        m_systemClock.Initialize();
    }

}
