using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using HierarchicalStateMachine.Definitions;
using HierarchicalStateMachine.Helpers;

namespace HierarchicalStateMachine.Engine
{
    public class CompiledState<TState, TInput, TOutput> where TState:notnull
    {
        private readonly StateMachineDef<TState, TInput, TOutput> _stateMachineDef;
        private readonly List<(TState to, Predicate<TInput> predicate)> _transitions;
        private readonly Dictionary<TState, List<Func<TInput, TOutput>>> _transitionFuncs;
        private readonly TState _id;

        public CompiledState(StateDef<TState, TInput, TOutput> def, StateMachineDef<TState, TInput, TOutput> stateMachineDef)
        {
            _stateMachineDef = stateMachineDef;
            _id = def.Id;
            _transitions = new List<(TState to, Predicate<TInput> predicate)>();
            _transitionFuncs = new Dictionary<TState, List<Func<TInput, TOutput>>>();
            CompileBehaviour(def);
            
        }

        public TState NextState(TInput input)
        {
            foreach (var transition in _transitions)
            {
                if (transition.predicate(input)) return transition.to;
            }
            return _id;
        }

        public IReadOnlyList<Func<TInput, TOutput>> TransitionFuncs(TState to) => 
            _transitionFuncs.GetValueOrDefault(to, new List<Func<TInput, TOutput>>());
        
        private void CompileBehaviour(StateDef<TState, TInput, TOutput> def)
        {
            CompileDefinedBehaviour();
            CompileInheritedBehavior();
            CompileOnStateBehaviour();

            void CompileDefinedBehaviour()
            {
                foreach (var definedTransition in def.Transitions)
                {
                    var targetStateDef = FindTargetState(definedTransition.To);
                    var effectiveTransition = new TransitionDef<TState, TInput>()
                    {
                        From = definedTransition.From,
                        To = targetStateDef.Id,
                        Predicate = definedTransition.Predicate
                    };
                    _transitions.Add((effectiveTransition.To, definedTransition.Predicate));
                    _transitionFuncs.AddOrUpdate(effectiveTransition.To, _ => CompileTransitionFuncs(effectiveTransition),
                        (_, l) => l.AddRange(CompileTransitionFuncs(effectiveTransition).ToArray()));
                }
            }

            void CompileInheritedBehavior()
            {
                foreach (var inheritedTransitionDef in def.InheritedTransitions())
                {
                    _transitions.Add((inheritedTransitionDef.To, inheritedTransitionDef.Predicate));
                    _transitionFuncs.AddOrUpdate(inheritedTransitionDef.To, _ => CompileTransitionFuncs(inheritedTransitionDef),
                        (_, l) => l.AddRange(CompileTransitionFuncs(inheritedTransitionDef).ToArray()));
                }
            }
            
            void CompileOnStateBehaviour()
            {
                _transitionFuncs.AddOrUpdate(def.Id, _ => OnStateFuncs(def.Id).ToList(),
                    (_, l) => l.AddRange(OnStateFuncs(def.Id)));
            }
        }

 
        private List<Func<TInput, TOutput>> CompileTransitionFuncs(TransitionDef<TState, TInput> transitionDef) => 
            OnExitFuncs(transitionDef.From)
            .Concat(OnEntryFuncs(transitionDef.To))
            .Concat(OnStateFuncs(transitionDef.To))
            .ToList();

        private IReadOnlyList<Func<TInput, TOutput>> OnExitFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var behaviourDef = stateDef?.InheritedBehaviour(def=>def.OnExit).Concat(stateDef?.OnExit);
            return behaviourDef?.Funcs.Reverse().ToArray() ?? Array.Empty<Func<TInput, TOutput>>();
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnEntryFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var behaviourDef = stateDef?.InheritedBehaviour(def=>def.OnEntry).Concat(stateDef?.OnEntry);
            return behaviourDef?.Funcs ?? Array.Empty<Func<TInput, TOutput>>();
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnStateFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var behaviourDef = stateDef?.InheritedBehaviour(def => def.OnState).Concat(stateDef?.OnState);
            return behaviourDef?.Funcs ?? Array.Empty<Func<TInput, TOutput>>();
        }
        
        private  StateDef<TState, TInput, TOutput> FindTargetState(TState stateId)
        {
            var def = _stateMachineDef.StateDef(stateId);
            Debug.Assert(def != null, nameof(def) + " != null");
            if (!def.IsComposite()) return def;
            var candidate = def.Children.Single(c => c.IsInitialSubState);
            return candidate.IsComposite() switch
            {
                false => candidate,
                true => FindTargetState(candidate.Id)
            };
        }
    }
}