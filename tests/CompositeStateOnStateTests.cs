using static NextMachina.Dsl.StateMachineDsl<NextMachina.Tests.State,int,int>;
using static NextMachina.Dsl.TransitionState<NextMachina.Tests.State,int>;

namespace NextMachina.Tests;

[TestFixture]
public class CompositeStateOnStateTests
{
    [TestCase(A_2,1,new []{10,200})]
    [TestCase(A_1_1,1,new []{10,100,1000})]
    public void should_execute_functions_defined_on_the_parent(State fromState, int input, int[] expected)
    {
        var sm = CreateStateMachine();
        var (_, outputs) = sm.TransitionFrom(fromState, input);
        Check.That(outputs).ContainsExactly(expected);
    }

    [Test]
    public void self_transition_compile_on_state_behaviour()
    {
        var sm = CreateStateMachineWithSelfTransition();
        var (nextState, outputs) = sm.TransitionFrom(A, 1);
        Check.That(nextState).IsEqualTo(A);
        Check.That(outputs).ContainsExactly(10, 10);
    }
        
    private static IStateMachine<State, int, int> CreateStateMachine()
    {
        return StateMachine(initialState:O)
            .State(O)
            .Transitions(
                To(A).When(i=>i==0)
            )
            .State(A)
            .OnState(i=>i*10)
            .Transitions(
                To(O).When(i=>i==0)
            )
            .State(A_1).InitialSubStateOf(A)
            .OnState(i=>i * 100)
            .State(A_1_1).InitialSubStateOf(A_1)
            .OnState(i=>i * 1000)
            .State(A_2).SubStateOf(A)
            .OnState(i=>i*200)
            .BuildDefinition()
            .Create();
    }

    private static IStateMachine<State, int, int> CreateStateMachineWithSelfTransition()
    {
        return StateMachine(A)
            .State(A)
            .OnState(i=>i*10)
            .Transitions(
                To(A).When(i=>i==1)
            )
            .BuildDefinition()
            .Create();
    }
}