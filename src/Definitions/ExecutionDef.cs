using System;
using System.Collections.Generic;

namespace SimpleStateMachine.Definitions
{
    public class ExecutionDef<TInput, TOutput>
    {
        public ExecutionDef(IReadOnlyList<Func<TInput, TOutput>> funcs)
        {
            Funcs = funcs;
        }

        public IReadOnlyList<Func<TInput, TOutput>> Funcs { get; }
    }
}