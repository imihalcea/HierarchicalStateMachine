using System.Linq;
using NUnit.Framework;
using SimpleStateMachine.Engine;
using static SimpleStateMachine.Dsl.StateMachineDsl<SimpleStateMachine.Tests.State,int,int>;
using static SimpleStateMachine.Dsl.TransitionState<SimpleStateMachine.Tests.State,int>;
using static SimpleStateMachine.Tests.State;

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
        public void state_changes_when_predicate_is_matching(State from, int input, State expected)
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

        [TestCase(A, 2, B, new[]{20,200,6,8})]
        [TestCase(A, 3, A, new[]{6})]
        [TestCase(B, 3, B, new[]{9,12})]
        public void execute_transition_functions(State currentState, int input, State expectedState, int[] expectedOutput)
        {
            var sm = StateMachine(A)
                .State(A)
                    .OnExit(i=>i*10)
                    .OnState(i=>i*2)
                    .Transitions(
                        To(B).When(i=>i>=0 && i%2==0)
                    )
                .State(B)
                    .OnEntry(i=>i*100)
                    .OnState(i=>i*3, i=>i*4)
                    .Transitions(
                        To(A).When(i=>i==42)
                    )
                .BuildDefinition()
                .Create();
            
            var (nextState, outputs) = sm.TransitionFrom(currentState, input);
            Assert.AreEqual(expectedState,nextState);
            Assert.True(expectedOutput.SequenceEqual(outputs));
        }
    }
}