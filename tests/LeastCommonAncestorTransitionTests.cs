using NUnit.Framework;
using HierarchicalStateMachine.Engine;
using static HierarchicalStateMachine.Dsl.StateMachineDsl<HierarchicalStateMachine.Tests.State,int,string>;
using static HierarchicalStateMachine.Dsl.TransitionState<HierarchicalStateMachine.Tests.State,int>;
using static HierarchicalStateMachine.Tests.State;

namespace HierarchicalStateMachine.Tests
{
    [TestFixture]
    public class LeastCommonAncestorTransitionTests
    {
        [Test]
        public void should_not_exit_parent_when_transitioning_between_siblings()
        {
            // Arrange
            // Parent state P has two children: A and B.
            // Transition from A to B should NOT trigger P.OnExit nor P.OnEntry.
            var sm = CreateStateMachine();

            // Act
            // Initial state is A (substate of P). We transition to B (sibling of A).
            var (nextState, outputs) = sm.TransitionFrom(A, 1);

            // Assert
            Assert.That(nextState, Is.EqualTo(B));
            
            // Expected behavior for standard HSM:
            // Exit A -> Entry B.
            // Parent P is NOT exited and NOT re-entered.
            
            // Current implementation behavior (likely to fail if not optimized):
            // Exit A -> Exit P -> Entry P -> Entry B
            
            var expected = new[] { "Exit A", "Entry B" };
            Assert.That(outputs, Is.EqualTo(expected));
        }

        private static IStateMachine<State, int, string> CreateStateMachine()
        {
            // Using State.C as a Parent container for this test to avoid conflict with other tests using A/B structure differently if needed,
            // but here I will reuse A and B as children of a parent P (let's map P to State.C for this test context).
            
            // Mapping:
            // C = Parent
            // A = Child 1
            // B = Child 2
            
            return StateMachine(A)
                .State(C) // Parent
                    .OnEntry(_ => "Entry Parent")
                    .OnExit(_ => "Exit Parent")
                
                .State(A).InitialSubStateOf(C) // Child 1
                    .OnEntry(_ => "Entry A")
                    .OnExit(_ => "Exit A")
                    .Transitions(
                        To(B).When(i => i == 1)
                    )
                
                .State(B).SubStateOf(C) // Child 2
                    .OnEntry(_ => "Entry B")
                    .OnExit(_ => "Exit B")
                
                .BuildDefinition()
                .Create();
        }
    }
}