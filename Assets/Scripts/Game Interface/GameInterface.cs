using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameInterfaceElements.Header;

public class GameInterface : MonoBehaviour {

    [Header("Major Elements")]
    [SerializeField, RedIfEmpty] InputConsole m_console;
    [SerializeField, RedIfEmpty] TaskQueue m_queue;
    [SerializeField, RedIfEmpty] CoreDisplay m_coresDisplay;

    [Header("Minor Elements")]
    [SerializeField, RedIfEmpty] StatusDisplay m_statusDisplay;
    [SerializeField, RedIfEmpty] CurrencyDisplay m_currencyDisplay;
    [SerializeField, RedIfEmpty] SystemClock m_systemClock;

    public void Initialize () {
        m_console.Initialize();
        m_queue.Initialize();
        m_coresDisplay.Initialize();
        // init all the things here
        // so they can reference each other

        m_statusDisplay.Initialize();
        m_currencyDisplay.Initialize();
        m_systemClock.Initialize();
    }

}
