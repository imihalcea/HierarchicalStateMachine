using System.Diagnostics;
using System.Linq;
using SimpleStateMachine.Definitions;

namespace SimpleStateMachine.Engine
{
    internal static class Compiler
    {
        internal static StateDef<TState, TInput, TOutput> InitialState<TState, TInput, TOutput>(
            StateDef<TState, TInput, TOutput>? def)
        {
            Debug.Assert(def != null, nameof(def) + " != null");
            if (!def.IsComposite()) return def;
            var candidate = def.Children.Single(c => c.IsInitialSubState);
            return candidate.IsComposite() switch
            {
                false => candidate,
                true => InitialState(candidate)
            };
        }
    }

    public static class DslExtensions
    {
        public static IStateMachine<TState, TInput, TOutput> Create<TState, TInput, TOutput>(
            this StateMachineDef<TState, TInput, TOutput> @this)
        {
            var states = new CompiledStates<TState, TInput, TOutput>(@this);
            return new StateMachine<TState, TInput, TOutput>(states);
        }
    }
}