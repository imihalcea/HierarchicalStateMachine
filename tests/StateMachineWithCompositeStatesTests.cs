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
        public void transition_to_a_simple_composite_state()
        {
            var sm = CreateStateMachine();
            var (nextState, _) = sm.TransitionFrom(O, 0);
            Assert.AreEqual(A_1, nextState);
        }

        [Test]
        public void transition_to_a_nested_composite_state()
        {
            var sm = CreateStateMachine();
            var (nextState, _) = sm.TransitionFrom(O, 1);
            Assert.AreEqual(B_2, nextState);
        }
        
        [Test]
        public void transition_to_a_nested_composite_state2()
        {
            var sm = CreateStateMachine();
            var (nextState, _) = sm.TransitionFrom(B_2, 43);
            Assert.AreEqual(B_1_1, nextState);
        }
        
        private static IStateMachine<State, int, int> CreateStateMachine()
        {
            return StateMachine(O)
                .State(O)
                    .Transitions(
                        To(A).When(i=>i==0),
                        To(B).When(i=>i==1)
                        )
                .State(A)
                    .OnExit(i=>i*10)
                    .OnState(i=>i*2)
                    .Transitions(
                        To(B).When(i=>i>=0 && i%2==0)
                    )
                .State(A_1).InitialSubStateOf(A)
                    .Transitions(To(A_2).When(i=>i==-2))
                .State(A_2).SubStateOf(A)
                    .Transitions(To(A_1).When(i=>i==-1))
                .State(B)
                    .Transitions(
                        To(A).When(i=>i==42)
                    )
                .State(B_2).InitialSubStateOf(B)
                    .Transitions(To(B_1).When(i=>i==43))
                .State(B_1).SubStateOf(B)
                    .State(B_1_1).InitialSubStateOf(B_1)
                    .State(B_1_2).SubStateOf(B_1)
                .BuildDefinition()
                .Create();
        }
    }
}