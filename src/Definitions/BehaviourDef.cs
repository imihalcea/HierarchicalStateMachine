using System;
using System.Collections.Generic;
using System.Linq;

namespace NextMachina.Definitions;

public class BehaviourDef<TInput, TOutput>(IReadOnlyList<Func<TInput, TOutput>> funcs)
{
    public IReadOnlyList<Func<TInput, TOutput>> Funcs { get; } = funcs;

    public BehaviourDef<TInput, TOutput> Concat(BehaviourDef<TInput, TOutput>? executionDef)
    {
        return new BehaviourDef<TInput, TOutput>(
            Funcs.Concat(executionDef?.Funcs??[]).ToArray());
    }
}