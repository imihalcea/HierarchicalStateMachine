using System;
using System.Collections.Generic;
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
            foreach (var transitionDef in def.Transitions)
            {
                _transitions.Add((transitionDef.To, transitionDef.Predicate));
                _transitionFuncs.AddOrUpdate(transitionDef.To,_=>CompileTransitionFuncs(transitionDef), (_,l)=>l.AddRange(CompileTransitionFuncs(transitionDef).ToArray()));
            }
            
            _transitionFuncs.AddOrUpdate(def.Id, _ => OnStateFuncs(def.Id).ToList(), (_,l)=>l.AddRange(OnStateFuncs(def.Id)));
        }

        private List<Func<TInput, TOutput>> CompileTransitionFuncs(TransitionDef<TState, TInput> transitionDef) => 
            OnExitFuncs(transitionDef.From)
            .Concat(OnEntryFuncs(transitionDef.To))
            .Concat(OnStateFuncs(transitionDef.To))
            .ToList();

        private IReadOnlyList<Func<TInput, TOutput>> OnExitFuncs(TState stateId) => 
            _stateMachineDef.StateDef(stateId)?.OnExit.Funcs ?? Array.Empty<Func<TInput,TOutput>>();
        
        private IReadOnlyList<Func<TInput, TOutput>> OnEntryFuncs(TState stateId) => 
            _stateMachineDef.StateDef(stateId)?.OnEntry.Funcs ?? Array.Empty<Func<TInput,TOutput>>();
        
        private IReadOnlyList<Func<TInput, TOutput>> OnStateFuncs(TState stateId) => 
            _stateMachineDef.StateDef(stateId)?.OnState.Funcs ?? Array.Empty<Func<TInput,TOutput>>();

    }
}