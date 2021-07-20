using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStateMachine.Definitions
{
    public class ExecutionDef<TInput, TOutput>
    {
        public ExecutionDef(IReadOnlyList<Func<TInput, TOutput>> funcs)
        {
            Funcs = funcs;
        }

        public IReadOnlyList<Func<TInput, TOutput>> Funcs { get; }

        public ExecutionDef<TInput, TOutput> Concat(ExecutionDef<TInput, TOutput>? executionDef)
        {
            return new(Funcs.Concat(executionDef?.Funcs??Array.Empty<Func<TInput,TOutput>>()).ToArray());
        }
    }
}