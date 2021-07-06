using NUnit.Framework;
using SimpleStateMachine.Engine;
using static SimpleStateMachine.Dsl.StateMachineDsl<SimpleStateMachine.Tests.StateMachineTransitionTests.State,int,int>;
using static SimpleStateMachine.Dsl.TransitionState<SimpleStateMachine.Tests.StateMachineTransitionTests.State,int>;
using static SimpleStateMachine.Tests.StateMachineTransitionTests.State;

namespace SimpleStateMachine.Tests
{
    public class StateMachineTransitionTests
    {
        [TestCase(A, 2, B)]
        [TestCase(A, 11, B)]
        [TestCase(A, -1, A)]
        [TestCase(A, 3, C)]
        [TestCase(C, 0, C)]
        [TestCase(C, 84, A)]
        [TestCase(B, 42, A)]
        [TestCase(B, 0, B)]
        public void transition(State from, int input, State expected)
        {
            var sm = StateMachine(A)
                        .State(A)
                            .Transitions(
                                    To(B).When(i=>i>=0 && i%2==0),
                                    To(B).When(i=>i==11),
                                    To(C).When(i=>i>=0 && i%2!=0)
                            )
                        .State(B)
                            .Transitions(
                                To(A).When(i=>i==42)
                            )
                        .State(C)
                            .Transitions(
                                To(A).When(i=>i==84)
                            )
                        .BuildDefinition()
                        .Create();
            
            var (nextState, _) = sm.TransitionFrom(from, input);
            Assert.AreEqual(expected,nextState);
        }
        
        public enum State
        {
            A,B,C
        }
    }
}