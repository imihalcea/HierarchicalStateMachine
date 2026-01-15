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

        public CompiledState(
            StateDef<TState, TInput, TOutput> def, 
            StateMachineDef<TState, TInput, TOutput> stateMachineDef)
        {
            _stateMachineDef = stateMachineDef;
            _id = def.Id;
            _transitions = [];
            _transitionFuncs = new();
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
            _transitionFuncs.GetValueOrDefault(to, []);
        
        private void CompileBehaviour(StateDef<TState, TInput, TOutput> def)
        {
            CompileDefinedBehaviour();
            CompileInheritedBehavior();
            CompileOnStateBehaviour();
            return;

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
                    _transitionFuncs.AddOrUpdate(
                        key: effectiveTransition.To, 
                        add: _ => CompileTransitionFuncs(effectiveTransition),
                        update: (_, l) => l.AddRange(CompileTransitionFuncs(effectiveTransition).ToArray())
                    );
                }
            }

            void CompileInheritedBehavior()
            {
                foreach (var inheritedTransitionDef in def.InheritedTransitions())
                {
                    _transitions.Add((inheritedTransitionDef.To, inheritedTransitionDef.Predicate));
                    _transitionFuncs.AddOrUpdate(
                        key: inheritedTransitionDef.To, 
                        add: _ => CompileTransitionFuncs(inheritedTransitionDef),
                        update:(_, l) => l.AddRange(CompileTransitionFuncs(inheritedTransitionDef).ToArray())
                    );
                }
            }
            
            void CompileOnStateBehaviour()
            {
                _transitionFuncs.AddOrUpdate(
                    key: def.Id, 
                    add: _ => OnStateFuncs(def.Id).ToList(),
                    update: (_, l) => l.AddRange(OnStateFuncs(def.Id)));
            }
        }

 
        private List<Func<TInput, TOutput>> CompileTransitionFuncs(TransitionDef<TState, TInput> transitionDef)
        {
            var lca = FindLeastCommonAncestor(transitionDef.From, transitionDef.To);
            
            return OnExitFuncs(transitionDef.From, lca)
                .Concat(OnEntryFuncs(transitionDef.To, lca))
                .Concat(OnStateFuncs(transitionDef.To))
                .ToList();
        }

        private StateDef<TState, TInput, TOutput>? FindLeastCommonAncestor(TState fromId, TState toId)
        {
            if (EqualityComparer<TState>.Default.Equals(fromId, toId)) return null; // Self transition: exit all, enter all

            var fromDef = _stateMachineDef.StateDef(fromId);
            var toDef = _stateMachineDef.StateDef(toId);

            if (fromDef == null || toDef == null) return null;

            var fromAncestors = GetAncestors(fromDef);
            var toAncestors = GetAncestors(toDef);

            return fromAncestors.FirstOrDefault(a => toAncestors.Contains(a));
        }

        private static HashSet<StateDef<TState, TInput, TOutput>> GetAncestors(StateDef<TState, TInput, TOutput> def)
        {
            var ancestors = new HashSet<StateDef<TState, TInput, TOutput>>();
            var current = def.Parent;
            while (current != null)
            {
                ancestors.Add(current);
                current = current.Parent;
            }
            return ancestors;
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnExitFuncs(
            TState stateId, 
            StateDef<TState, TInput, TOutput>? stopAtAncestor = null)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            if (stateDef == null) return Array.Empty<Func<TInput, TOutput>>();

            var funcs = new List<Func<TInput, TOutput>>();
            
            // Collect OnExit from current state up to (but not including) the stopAtAncestor
            var current = stateDef;
            while (current != null && current != stopAtAncestor)
            {
                funcs.AddRange(current.OnExit.Funcs);
                current = current.Parent;
            }

            return funcs.ToArray();
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnEntryFuncs(
            TState stateId, 
            StateDef<TState, TInput, TOutput>? stopAtAncestor = null)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            if (stateDef == null) return Array.Empty<Func<TInput, TOutput>>();

            var funcs = new List<Func<TInput, TOutput>>();
            
            // Collect OnEntry from current state up to (but not including) the stopAtAncestor
            // Since we walk up the tree, we are collecting in reverse order of entry (Child -> Parent)
            // We need to reverse it later to get Parent -> Child
            var current = stateDef;
            while (current != null && current != stopAtAncestor)
            {
                funcs.AddRange(current.OnEntry.Funcs);
                current = current.Parent;
            }

            // Reverse to execute from top-most parent down to child
            funcs.Reverse();
            return funcs.ToArray();
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnStateFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var behaviourDef = stateDef?.InheritedBehaviour(def => def.OnState).Concat(stateDef.OnState);
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