using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SimpleStateMachine.Definitions;

namespace SimpleStateMachine.Dsl
{
    public class StateDsl<TState, TInput, TOutput>
    {
        private readonly TState _stateId;
        
        private readonly StateMachineDsl<TState, TInput, TOutput> _stateMachineDsl;


        public StateDsl(TState stateId, StateMachineDsl<TState, TInput, TOutput> stateMachineDsl)
        {
            _stateId = stateId;
            _stateMachineDsl = stateMachineDsl;
            MyStateDef = new StateDef<TState, TInput, TOutput>(stateId);
            _stateMachineDsl.AddState(MyStateDef);

        }

        internal StateDef<TState,TInput,TOutput> MyStateDef { get; }

        public StateDsl<TState, TInput, TOutput> State(TState stateId) => new(stateId,_stateMachineDsl);

        public StateDsl<TState, TInput, TOutput> OnEntry(params Func<TInput, TOutput>[] funcs)
        {
            MyStateDef.OnEntry = new ExecutionDef<TInput, TOutput>(funcs);
            return this;
        }
        
        public StateDsl<TState, TInput, TOutput> OnState(params Func<TInput, TOutput>[] funcs)
        {
            MyStateDef.OnState = new ExecutionDef<TInput, TOutput>(funcs);
            return this;
        }
        
        public StateDsl<TState, TInput, TOutput> OnExit(params Func<TInput, TOutput>[] funcs)
        {
            MyStateDef.OnExit = new ExecutionDef<TInput, TOutput>(funcs);
            return this;
        }
        
        public StateDsl<TState, TInput, TOutput> Transitions(params TransitionDsl<TState, TInput>[] transitions)
        {
            MyStateDef.Transitions = transitions.Select(t => new TransitionDef<TState, TInput>()
                {From = MyStateDef.Id, To = t.TargetState, Predicate = t.Predicate}).ToList().AsReadOnly();
            return this;
        }

        public StateMachineDef<TState,TInput,TOutput> BuildDefinition()
        {
            var initialState = _stateMachineDsl._definedStates.First(s => s.Id.Equals(_stateMachineDsl._initialStateId));
            return new StateMachineDef<TState, TInput, TOutput>(initialState, _stateMachineDsl._definedStates);
        }
    }
}