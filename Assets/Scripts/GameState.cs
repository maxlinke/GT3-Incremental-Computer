using System.Collections.Generic;
using UnityEngine;
using Cores;
using Tasks;

[System.Serializable]
public class GameState {

    public const string CURRENCY_SYMBOL = "CR";

    public GameState () {
        m_currency = 0;
        m_running = false;
        m_idQueue = ID.GetNewIDQueue();
        m_tasks = new List<Tasks.Task>();
        m_cores = new Core[CoreDisplay.NUMBER_OF_CORES];
        m_cores[0] = Core.GetInitialCore(() => ID.GetNext(m_idQueue));
        for(int i=1; i<m_cores.Length; i++){
            m_cores[i] = new Core();
        }
    }

    private static void OnValueSet<T> (GameState state, T value, ref T fieldValue, System.Action<T> onUpdated) {
        if(!value.Equals(fieldValue)){
            fieldValue = value;
            if(state.Equals(current)){
                onUpdated(value);
            }
        }
    }

    private static GameState _current = new GameState();
    public static GameState current {
        get => _current;
        set => OnValueSet(value, value, ref _current, (gs) => {onGameStateChanged(gs); Debug.Log("gs change");});
    }

    public static event System.Action<GameState> onGameStateChanged = delegate {};
    public static event System.Action<int> onCurrencyChanged = delegate {};
    public static event System.Action<bool> onRunStateChanged = delegate {};

    [SerializeField] private int m_currency;
    [SerializeField] private bool m_running;
    [SerializeField] private bool m_hasRun;
    [SerializeField] private List<ID> m_idQueue;
    [SerializeField] private List<Task> m_tasks;
    [SerializeField] private Core[] m_cores;

    public int currency {
        get => m_currency;
        set => OnValueSet(this, value, ref m_currency, onCurrencyChanged);
    }

    public bool running {
        get => m_running;
        set { OnValueSet(this, value, ref m_running, onRunStateChanged); m_hasRun |= value; }
    }

    public bool hasRun => m_hasRun;
    public IList<Task> tasks => m_tasks;
    public IReadOnlyList<Core> cores => m_cores;
    public IList<ID> idQueue => m_idQueue;

}
