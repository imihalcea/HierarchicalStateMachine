using System;
using SimpleStateMachine.Definitions;

namespace SimpleStateMachine.Engine
{
    internal class StateMachine<TState, TInput, TOutput> : IStateMachine<TState, TInput, TOutput>
    {
        private readonly CompiledStates<TState, TInput, TOutput> _states;

        internal StateMachine(CompiledStates<TState, TInput, TOutput> states)
        {
            _states = states;
        }

        public (TState nextState, TOutput[] outputs) TransitionFrom(TState state, TInput input)
        {
            return (_states.NextState(state, input), Array.Empty<TOutput>());
        }
    }
}
