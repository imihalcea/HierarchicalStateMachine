using NUnit.Framework;
using SimpleStateMachine.Engine;
using static SimpleStateMachine.Dsl.StateMachineDsl<SimpleStateMachine.Tests.State,int,int>;
using static SimpleStateMachine.Dsl.TransitionState<SimpleStateMachine.Tests.State,int>;
using static SimpleStateMachine.Tests.State;

namespace SimpleStateMachine.Tests
{
    public class StateMachineWithCompositeStatesTests
    {
        [Test]
        public void transition_to_composite_state()
        {
            var sm = CreateStateMachine();
            var (nextState, _) = sm.TransitionFrom(O, 0);
            Assert.AreEqual(A1, nextState);
        }

        private static IStateMachine<State, int, int> CreateStateMachine()
        {
            
            return StateMachine(O)
                .State(O)
                    .Transitions(To(A).When(i=>i==0))
                .State(A)
                    .OnExit(i=>i*10)
                    .OnState(i=>i*2)
                    .Transitions(
                        To(B).When(i=>i>=0 && i%2==0)
                    )
                .State(A1).InitialSubStateOf(A)
                    .Transitions(To(A2).When(i=>i==-2))
                .State(A2).SubStateOf(A)
                    .Transitions(To(A1).When(i=>i==-1))
                .State(B)
                    .OnEntry(i=>i*100)
                    .OnState(i=>i*3, i=>i*4)
                    .Transitions(
                        To(A).When(i=>i==42)
                    )
                .BuildDefinition()
                .Create();
        }
    }
}