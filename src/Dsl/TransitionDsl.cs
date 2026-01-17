using System;

namespace NextMachina.Dsl;

public class TransitionState<TState, TInput>
{
    private readonly TState _targetState;

    private TransitionState(TState targetState)
    {
        _targetState = targetState;
    }

    public static TransitionState<TState, TInput> To(TState targetState)
    {
        return new TransitionState<TState, TInput>(targetState);
    }

    public TransitionDsl<TState, TInput> When(Predicate<TInput> predicate) => 
        new(_targetState, predicate);
}
    
public class TransitionDsl<TState, TInput>(TState targetState, Predicate<TInput> predicate)
{
    public TState TargetState { get; } = targetState;
    public Predicate<TInput> Predicate { get; } = predicate;
}