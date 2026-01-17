using NextMachina.Definitions;

namespace NextMachina.Engine;

public static class DslExtensions
{
    public static IStateMachine<TState, TInput, TOutput> Create<TState, TInput, TOutput>(
        this StateMachineDef<TState, TInput, TOutput> @this) where TState:notnull
    {
        var states = new CompiledStates<TState, TInput, TOutput>(@this);
        return new StateMachine<TState, TInput, TOutput>(states);
    }
}