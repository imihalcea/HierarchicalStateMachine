namespace HierarchicalStateMachine
{
    public interface IStateMachine<TState, in TInput, TOutput>
    {
        (TState nextState, TOutput[] outputs) TransitionFrom(TState state, TInput input);
    }
}