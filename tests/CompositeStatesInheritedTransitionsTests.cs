using static NextMachina.Dsl.StateMachineDsl<NextMachina.Tests.State,int,int>;
using static NextMachina.Dsl.TransitionState<NextMachina.Tests.State,int>;

namespace NextMachina.Tests;

public class CompositeStatesInheritedTransitionsTests
{
    [TestCase(A_1_1,1,O)]
    [TestCase(A_2,1,O)]
    public void should_inherit_transitions_defined_on_parent(State fromState, int input, State expectedState)
    {
        var sm = CreateStateMachine();
        var (nextState, _) = sm.TransitionFrom(fromState, input);
        Check.That(nextState).IsEqualTo(expectedState);
    }
        
    private static IStateMachine<State, int, int> CreateStateMachine()
    {
        return StateMachine(O)
            .State(O)
            .Transitions(
                To(A).When(i=>i==1)
            )
            .State(A)
            .OnExit(i=>i*10)
            .Transitions(
                To(O).When(i=>i==1)
            )
            .State(A_1).InitialSubStateOf(A)
            .State(A_1_1).InitialSubStateOf(A_1)
            .State(A_2).SubStateOf(A)
            .BuildDefinition()
            .Create();
    }
}