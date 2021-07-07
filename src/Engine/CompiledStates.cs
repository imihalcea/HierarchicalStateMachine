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
                _compiledStates.Add(stateDef.Id, new CompiledState<TState, TInput, TOutput>(stateDef, stateMachineDef));
            }
        }

        public (TState,IReadOnlyList<Func<TInput,TOutput>>) NextState(TState from, TInput input)
        {
            Debug.Assert(_compiledStates.ContainsKey(from),$"No transitions defined for state {from.ToString()}");
            var nextState = _compiledStates[@from].NextState(input);
            var funcs = _compiledStates[@from].TransitionFuncs(nextState);
            return (nextState,funcs);
        }
    }
}