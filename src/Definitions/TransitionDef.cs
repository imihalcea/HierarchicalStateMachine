using System;

namespace SimpleStateMachine.Definitions
{
    public class TransitionDef<TState, TInput> where TState:notnull
    {
        public TState From { get; internal set; }
        public TState To { get; internal set; }
        public Predicate<TInput> Predicate { get; internal set; }
    }
}