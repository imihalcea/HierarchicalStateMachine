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
        [Test]
        public void should_execute_functions_defined_on_the_parent()
        {
            var sm = CreateStateMachine();
            var (nextState, outputs) = sm.TransitionFrom(A_1, 1);
            Check.That(nextState).IsEqualTo(A_1);
            Check.That(outputs).ContainsExactly(10, 100);
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
                .BuildDefinition()
                .Create();
        }
    }
}