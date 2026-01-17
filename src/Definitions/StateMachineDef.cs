#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NextMachina.Definitions;

public class StateMachineDef<TState, TInput, TOutput> where TState:notnull
{
    private readonly Dictionary<TState,StateDef<TState,TInput,TOutput>> _states;

    public StateMachineDef(StateDef<TState, TInput, TOutput> initialState, IEnumerable<StateDef<TState, TInput, TOutput>> stateDefs)
    {
        _states = stateDefs.Aggregate(new Dictionary<TState, StateDef<TState, TInput, TOutput>>(), (dic, def) =>
        {
            dic.Add(def.Id, def);
            return dic;
        });
        InitialState = initialState;
        States = new HashSet<StateDef<TState, TInput, TOutput>>(_states.Values);
    }

    public StateDef<TState, TInput, TOutput> InitialState { get; internal set; }

    public HashSet<StateDef<TState, TInput, TOutput>> States { get;  }

    public StateDef<TState, TInput, TOutput>? StateDef(TState stateId)
    {
        return _states!.GetValueOrDefault(stateId, null);
    }
}