using System;
using System.Collections.Generic;
using System.Diagnostics;
using SimpleStateMachine.Definitions;

namespace SimpleStateMachine.Engine
{
    public class CompiledStates<TState, TInput, TOutput>
    {
        private readonly Dictionary<TState,CompiledState<TState,TInput,TOutput>> _compiledStates;

        public CompiledStates(StateMachineDef<TState, TInput, TOutput> stateMachineDef)
        {
            _compiledStates = new Dictionary<TState, CompiledState<TState, TInput, TOutput>>();
            foreach (var stateDef in stateMachineDef.States)
            {
                _compiledStates.Add(stateDef.Id, new CompiledState<TState, TInput, TOutput>(stateDef));
            }
        }

        public TState NextState(TState from, TInput input)
        {
            Debug.Assert(_compiledStates.ContainsKey(from),$"No transitions defined for state {from.ToString()}");
            return _compiledStates[from].NextState(input);
        }
    }

    public class CompiledState<TState, TInput, TOutput>
    {
        private readonly List<(TState to, Predicate<TInput> predicate)> _transitions;
        private readonly TState _id;

        public CompiledState(StateDef<TState, TInput, TOutput> def)
        {
            _id = def.Id;
            _transitions = new List<(TState to, Predicate<TInput> predicate)>();
            foreach (var transitionDef in def.Transitions)
            {
                _transitions.Add((transitionDef.To,transitionDef.Predicate));
            }
            
        }

        public TState NextState(TInput input)
        {
            foreach (var transition in _transitions)
            {
                if (transition.predicate(input)) return transition.to;
            }
            return _id;
        }
    }
}