using System.Collections.Generic;
using System.Linq;
using NextMachina.Definitions;

namespace NextMachina.Dsl;

public class StateMachineDsl<TState, TInput, TOutput> where TState:notnull
{
    internal readonly TState InitialStateId;
    internal readonly List<StateDef<TState,TInput,TOutput>> DefinedStates;

    private StateMachineDsl(TState initialState)
    {
        InitialStateId = initialState;
        DefinedStates = new List<StateDef<TState, TInput, TOutput>>();
    }

    public static StateMachineDsl<TState, TInput, TOutput> StateMachine(TState initialState)
    {
        return new(initialState);
    }

    public StateDsl<TState, TInput, TOutput> State(TState stateId)
    {
        return new(stateId, this);
    }
        

    internal void AddState(StateDef<TState,TInput,TOutput> stateDef)
    {
        DefinedStates.Add(stateDef);
    }

    internal StateDef<TState, TInput, TOutput>? GetDefinedState(TState stateId)
    {
        return DefinedStates.FirstOrDefault(s => stateId!.Equals(s.Id));
    }


}