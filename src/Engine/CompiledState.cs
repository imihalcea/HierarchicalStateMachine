using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using SimpleStateMachine.Definitions;
using SimpleStateMachine.Helpers;

namespace SimpleStateMachine.Engine
{
    public class CompiledState<TState, TInput, TOutput>
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
            CompileTransitions(def);
            
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
        
        private void CompileTransitions(StateDef<TState, TInput, TOutput> def)
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
                _transitionFuncs.AddOrUpdate(effectiveTransition.To,_=>CompileTransitionFuncs(effectiveTransition), (_,l)=>l.AddRange(CompileTransitionFuncs(effectiveTransition).ToArray()));
            }

            ExecuteWhenNotTransitioning(def);
        }

        private void ExecuteWhenNotTransitioning(StateDef<TState, TInput, TOutput> def)
        {
            _transitionFuncs.AddOrUpdate(def.Id, _ => OnStateFuncs(def.Id).ToList(),
                (_, l) => l.AddRange(OnStateFuncs(def.Id)));
        }

        private List<Func<TInput, TOutput>> CompileTransitionFuncs(TransitionDef<TState, TInput> transitionDef) => 
            OnExitFuncs(transitionDef.From)
            .Concat(OnEntryFuncs(transitionDef.To))
            .Concat(OnStateFuncs(transitionDef.To))
            .ToList();

        private IReadOnlyList<Func<TInput, TOutput>> OnExitFuncs(TState stateId) => 
            _stateMachineDef.StateDef(stateId)?.OnExit.Funcs ?? Array.Empty<Func<TInput,TOutput>>();
        
        private IReadOnlyList<Func<TInput, TOutput>> OnEntryFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var onEntryFuncs = stateDef?.Inherited(def=>def.OnEntry).Concat(stateDef?.OnEntry);
            return onEntryFuncs?.Funcs ?? Array.Empty<Func<TInput, TOutput>>();
        }

        private IReadOnlyList<Func<TInput, TOutput>> OnStateFuncs(TState stateId)
        {
            var stateDef = _stateMachineDef.StateDef(stateId);
            var onStateFuncs = stateDef?.Inherited(def => def.OnState).Concat(stateDef?.OnState);
            return onStateFuncs?.Funcs ?? Array.Empty<Func<TInput, TOutput>>();
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