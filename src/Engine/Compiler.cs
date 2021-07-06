using System;
using System.Linq;
using SimpleStateMachine.Definitions;

namespace SimpleStateMachine.Engine
{
    internal class Compiler
    {
        public IStateMachine<TState, TInput, TOutput> Compile<TState, TInput, TOutput>(StateMachineDef<TState, TInput, TOutput> def)
        {
            var states = new CompiledStates<TState, TInput, TOutput>(def);
            return new StateMachine<TState, TInput, TOutput>(states);
        }
    }

    public static class DslExtensions
    {
        public static IStateMachine<TState, TInput, TOutput> Create<TState, TInput, TOutput>(this StateMachineDef<TState, TInput, TOutput> @this)
        {
            var compiler = new Compiler();
            return compiler.Compile(@this);
        }
    }
}