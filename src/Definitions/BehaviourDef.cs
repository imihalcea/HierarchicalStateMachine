using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStateMachine.Definitions
{
    public class BehaviourDef<TInput, TOutput>
    {
        public BehaviourDef(IReadOnlyList<Func<TInput, TOutput>> funcs)
        {
            Funcs = funcs;
        }

        public IReadOnlyList<Func<TInput, TOutput>> Funcs { get; }

        public BehaviourDef<TInput, TOutput> Concat(BehaviourDef<TInput, TOutput>? executionDef)
        {
            return new(Funcs.Concat(executionDef?.Funcs??Array.Empty<Func<TInput,TOutput>>()).ToArray());
        }
    }
}