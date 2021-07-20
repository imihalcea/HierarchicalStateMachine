using System;
using NFluent;
using NUnit.Framework;
using SimpleStateMachine.Engine;
using static SimpleStateMachine.Dsl.StateMachineDsl<SimpleStateMachine.Tests.State,int,int>;
using static SimpleStateMachine.Dsl.TransitionState<SimpleStateMachine.Tests.State,int>;
using static SimpleStateMachine.Tests.State;

namespace SimpleStateMachine.Tests
{
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
        
        
        private static IStateMachine<State, int, int> CreateStateMachine()
        {
            return StateMachine(O)
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
    }
}