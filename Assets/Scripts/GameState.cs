using System.Collections.Generic;
using UnityEngine;
using Cores;
using Cores.Components;
using Tasks;

[System.Serializable]
public class GameState {

    public const string CURRENCY_SYMBOL = "CR";
    public const char UPGRADE_SYMBOL = '*';
    public const float DEFAULT_TEMPERATURE = 21;

    public static string GetUpgradeIndicator (int upgrades) {
        return new string(UPGRADE_SYMBOL, upgrades);
    }

    public GameState () {
        m_currency = 0;
        m_running = false;
        m_idQueue = ID.CreateNewIDQueue();
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
        set => OnValueSet(value, value, ref _current, onGameStateChanged);
    }

    public static event System.Action<GameState> onGameStateChanged = delegate {};
    public static event System.Action<int> onCurrencyChanged = delegate {};
    public static event System.Action<bool> onRunStateChanged = delegate {};

    [SerializeField] private int m_currency;
    [SerializeField] private bool m_running;
    [SerializeField] private bool m_hasRun;
    [SerializeField] private List<ID> m_idQueue;
    [SerializeField] private List<Task> m_tasks;
    [SerializeField] private int m_taskLevel;
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
    public int taskLevel { 
        get => m_taskLevel;
        set => m_taskLevel = Mathf.Clamp(value, 0, Task.MAX_LEVEL);
    }
    public IReadOnlyList<Core> cores => m_cores;
    public IList<ID> idQueue => m_idQueue;

    public bool TryFindComponentForId (string id, out CoreComponent output) {
        foreach(var core in GameState.current.cores){
            foreach(var component in core.components){
                if(component.id.Equals(id)){
                    output = component;
                    return true;
                }
            }
        }
        output = default;
        return false;
    }

    public CoreComponent GetCoreComponentForId (string id) {
        if(TryFindComponentForId(id, out var output)){
           return output; 
        }
        return default;
    }

    public bool TryFindCoreForIndex (string index, out Core core) {
        var goodParse = int.TryParse(index, out var coreIndex);
        if(goodParse){
            return TryFindCoreForIndex(coreIndex, out core);
        }
        core = default;
        return false;
    }

    public bool TryFindCoreForIndex (int index, out Core core) {
        try{
            core = GameState.current.cores[index];
        }catch(System.IndexOutOfRangeException){
            core = default;
        }catch(System.ArgumentOutOfRangeException){
            core = default;
        }
        return core != default;
    }

    public int GetTotalWealth () {
        var output = currency;
        foreach(var core in cores){
            foreach(var component in core.components){
                output += ShopDisplay.GetPriceOfComponent(component);
            }
        }
        return output;
    }

}
