using System;

namespace NextMachina.Definitions;

public class TransitionDef<TState, TInput> where TState:notnull
{
    public required TState From { get; set; }
    public required TState To { get; set; }
    public required Predicate<TInput> Predicate { get; set; }
}