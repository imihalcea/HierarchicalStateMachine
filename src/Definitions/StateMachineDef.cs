using System.Collections.Generic;
using System.Diagnostics;

namespace SimpleStateMachine.Definitions
{
    public class StateMachineDef<TState, TInput, TOutput>
    {
        public StateMachineDef(StateDef<TState, TInput, TOutput> initialState, IEnumerable<StateDef<TState, TInput, TOutput>> stateDefs)
        {
            InitialState = initialState;
            States = new HashSet<StateDef<TState, TInput, TOutput>>(stateDefs);
        }

        public StateDef<TState, TInput, TOutput> InitialState { get; internal set; }

        public HashSet<StateDef<TState, TInput, TOutput>> States { get;  }
    }
}