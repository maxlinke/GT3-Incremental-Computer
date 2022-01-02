using System.Collections.Generic;
using UnityEngine;
using UniRx;
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
        m_coreStates = new Core.State[CoreDisplay.NUMBER_OF_CORES];
        m_coreStates[0] = Core.State.GetNewStateForInitialCore(() => ID.GetNext(m_idQueue));
        for(int i=1; i<m_coreStates.Length; i++){
            m_coreStates[i] = new Core.State();
        }
    }

    private static void OnValueSet<T> (GameState state, T value, ref T fieldValue, Subject<T> onUpdated) {
        if(!value.Equals(fieldValue)){
            fieldValue = value;
            if(state.Equals(current)){
                onUpdated.OnNext(value);
            }
        }
    }

    private static GameState _current = new GameState();
    public static GameState current {
        get => _current;
        set => OnValueSet(value, value, ref _current, onGameStateChanged);
    }

    public static Subject<GameState> onGameStateChanged { get; private set; } = new Subject<GameState>();
    public static Subject<int> onCurrencyChanged { get; private set; } = new Subject<int>();
    public static Subject<bool> onRunStateChanged { get; private set; } = new Subject<bool>();

    [SerializeField] private int m_currency;
    [SerializeField] private bool m_running;
    [SerializeField] private bool m_hasRun;
    [SerializeField] private List<ID> m_idQueue;
    [SerializeField] private List<Task> m_tasks;
    [SerializeField] private Core.State[] m_coreStates;

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
    public IReadOnlyList<Core.State> coreStates => m_coreStates;
    public IList<ID> idQueue => m_idQueue;

}
