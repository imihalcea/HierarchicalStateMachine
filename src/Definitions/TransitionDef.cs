using System;

namespace NextMachina.Definitions;

public class TransitionDef<TState, TInput> where TState:notnull
{
    public required TState From { get; init; }
    public required TState To { get; init; }
    public required Predicate<TInput> Predicate { get; init; }
}