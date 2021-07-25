using System.Linq;

namespace SimpleStateMachine.Engine
{
    internal class StateMachine<TState, TInput, TOutput> : IStateMachine<TState, TInput, TOutput> where TState:notnull
    {
        private readonly CompiledStates<TState, TInput, TOutput> _states;

        internal StateMachine(CompiledStates<TState, TInput, TOutput> states)
        {
            _states = states;
        }

        public (TState nextState, TOutput[] outputs) TransitionFrom(TState state, TInput input)
        {
            var (nextState, funcs) = _states.NextState(state, input);
            return (nextState, funcs.Select(f=>f(input)).ToArray());
        }
    }
}
