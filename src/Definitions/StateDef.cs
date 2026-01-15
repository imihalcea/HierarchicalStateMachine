using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NextMachina.Definitions;

public class StateDef<TState, TInput, TOutput> : IEquatable<StateDef<TState, TInput, TOutput>> where TState:notnull
{
    private readonly List<StateDef<TState, TInput, TOutput>> _children;
    private StateDef<TState, TInput, TOutput>? _parent;
    private readonly List<TransitionDef<TState, TInput>> _transitions;

    public StateDef(TState id)
    {
        Id = id;
        _children = new List<StateDef<TState, TInput, TOutput>>();
        OnExit = new BehaviourDef<TInput, TOutput>([]);
        OnEntry = new BehaviourDef<TInput, TOutput>([]);
        OnState = new BehaviourDef<TInput, TOutput>([]);
        _transitions = new List<TransitionDef<TState, TInput>>();

    }

    public TState Id { get; internal set; }

    public bool IsInitialSubState { get; internal set; }

    public StateDef<TState, TInput, TOutput>? Parent
    {
        get => _parent;
        internal set
        {
            Debug.Assert(value != null, nameof(value) + " != null");
            _parent = value;
            _parent.AddChild(this);
        }
    }

    public IReadOnlyList<StateDef<TState, TInput, TOutput>> Children => _children.AsReadOnly();

    public StateDef<TState, TInput, TOutput> AddChild(StateDef<TState, TInput, TOutput> child)
    {
        _children.Add(child);
        return this;
    }

    public BehaviourDef<TInput, TOutput> OnEntry { get; internal set; }

    public BehaviourDef<TInput, TOutput> OnExit { get; internal set; }

    public BehaviourDef<TInput, TOutput> OnState { get; internal set; }

    public BehaviourDef<TInput, TOutput> InheritedBehaviour(Func<StateDef<TState, TInput, TOutput>, BehaviourDef<TInput, TOutput>> f)
    {
        return Parent switch
        {
            null => new BehaviourDef<TInput, TOutput>(new List<Func<TInput, TOutput>>()),
            _ => Parent.InheritedBehaviour(f).Concat(f(Parent))
        };
    }
        
    public IReadOnlyList<TransitionDef<TState,TInput>> InheritedTransitions()
    {
        var transitionDefs =  Parent switch
        {
            null => new List<TransitionDef<TState, TInput>>(),
            _ => Parent.Transitions.Concat(Parent.InheritedTransitions()).ToList()
        };

        return transitionDefs.Select(it => new TransitionDef<TState, TInput>()
        {
            From = Id,
            Predicate = it.Predicate,
            To = it.To
        }).ToList();
    }

    public IReadOnlyList<TransitionDef<TState, TInput>> Transitions => _transitions;

    public bool Equals(StateDef<TState, TInput, TOutput>? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EqualityComparer<TState>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((StateDef<TState, TInput, TOutput>) obj);
    }

    public override int GetHashCode()
    {
        return EqualityComparer<TState>.Default.GetHashCode(Id);
    }

    public static bool operator ==(StateDef<TState, TInput, TOutput>? left, StateDef<TState, TInput, TOutput>? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(StateDef<TState, TInput, TOutput>? left, StateDef<TState, TInput, TOutput>? right)
    {
        return !Equals(left, right);
    }

    public bool IsComposite()
    {
        return Children.Count > 0;
    }

    public void AddTransitions(List<TransitionDef<TState,TInput>> transitions)
    {
        _transitions.AddRange(transitions);
    }


}