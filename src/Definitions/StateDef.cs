using System;
using System.Collections.Generic;

namespace SimpleStateMachine.Definitions
{
    public class StateDef<TState, TInput, TOutput> : IEquatable<StateDef<TState, TInput, TOutput>>
    {
        private readonly List<StateDef<TState, TInput, TOutput>> _children;

        public StateDef(TState id)
        {
            Id = id;
            _children = new List<StateDef<TState, TInput, TOutput>>();
        }

        public TState Id { get; internal set; }
        public bool IsInitialSubState { get; internal set; }
        public StateDef<TState, TInput, TOutput> Parent { get; internal set; }

        public IReadOnlyList<StateDef<TState, TInput, TOutput>> Children => _children.AsReadOnly();

        public StateDef<TState, TInput, TOutput> AddChild(StateDef<TState, TInput, TOutput> child)
        {
            _children.Add(child);
            return this;
        }
        
        public ExecutionDef<TInput,TOutput> OnEntry { get; internal set; }
        public ExecutionDef<TInput,TOutput> OnExit { get; internal set; }
        public ExecutionDef<TInput,TOutput> OnState { get; internal set; }
        
        public IReadOnlyList<TransitionDef<TState,TInput>> Transitions { get; internal set; }

        public bool Equals(StateDef<TState, TInput, TOutput> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<TState>.Default.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
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

        public static bool operator ==(StateDef<TState, TInput, TOutput> left, StateDef<TState, TInput, TOutput> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StateDef<TState, TInput, TOutput> left, StateDef<TState, TInput, TOutput> right)
        {
            return !Equals(left, right);
        }
    }
}