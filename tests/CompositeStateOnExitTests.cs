using HierarchicalStateMachine.Engine;
using NFluent;
using NUnit.Framework;
using static HierarchicalStateMachine.Dsl.StateMachineDsl<HierarchicalStateMachine.Tests.State,int,int>;
using static HierarchicalStateMachine.Dsl.TransitionState<HierarchicalStateMachine.Tests.State,int>;
using static HierarchicalStateMachine.Tests.State;


namespace HierarchicalStateMachine.Tests
{
    public class CompositeStateOnExitTests
    {
        
        [TestCase(A_1_1,1,new []{1000,100,10})]
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
                .OnExit(i=>i*10)
                .Transitions(
                    To(O).When(i=>i==1)
                )
                .State(A_1).InitialSubStateOf(A)
                .OnExit(i=>i * 100)
                .State(A_1_1).InitialSubStateOf(A_1)
                .OnExit(i=>i * 1000)
                .State(A_2).SubStateOf(A)
                .OnEntry(i=>i*200)
                .BuildDefinition()
                .Create();
        }
    }
}