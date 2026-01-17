using static NextMachina.Dsl.StateMachineDsl<NextMachina.Tests.State,int,int>;
using static NextMachina.Dsl.TransitionState<NextMachina.Tests.State,int>;

namespace NextMachina.Tests;

[TestFixture]
public class CompositeStateOnEntryTests
{
    [TestCase(O,1,new []{10,100,1000})]
    public void should_execute_functions_defined_on_the_parent(State fromState, int input, int[] expected)
    {
        var sm = CreateStateMachine();
        var (ns, outputs) = sm.TransitionFrom(fromState, input);
        Check.That(outputs).ContainsExactly(expected);
    }
        
        
    private static IStateMachine<State, int, int> CreateStateMachine()
    {
        return StateMachine(O)
            .State(O)
            .Transitions(
                To(A).When(i=>i==1)
            )
            .State(A)
            .OnEntry(i=>i*10)
            .Transitions(
                To(O).When(i=>i==1)
            )
            .State(A_1).InitialSubStateOf(A)
            .OnEntry(i=>i * 100)
            .State(A_1_1).InitialSubStateOf(A_1)
            .OnEntry(i=>i * 1000)
            .State(A_2).SubStateOf(A)
            .OnEntry(i=>i*200)
            .BuildDefinition()
            .Create();
    }
}