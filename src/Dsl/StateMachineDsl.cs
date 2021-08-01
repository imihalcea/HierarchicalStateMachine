using System.Collections.Generic;
using System.Linq;
using HierarchicalStateMachine.Definitions;

namespace HierarchicalStateMachine.Dsl
{
    public class StateMachineDsl<TState, TInput, TOutput> where TState:notnull
    {
        internal readonly TState _initialStateId;
        internal readonly List<StateDef<TState,TInput,TOutput>> _definedStates;

        private StateMachineDsl(TState initialState)
        {
            _initialStateId = initialState;
            _definedStates = new List<StateDef<TState, TInput, TOutput>>();
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
           _definedStates.Add(stateDef);
        }

        internal StateDef<TState, TInput, TOutput>? GetDefinedState(TState stateId)
        {
            return _definedStates.FirstOrDefault(s => stateId!.Equals(s.Id));
        }


    }
}