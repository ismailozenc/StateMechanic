using System.Collections.Generic;
using System.Linq;

namespace StateMechanic
{
    internal struct ForcedTransitionInvoker<TState> : ITransitionInvoker<TState>
        where TState : StateBase<TState>, new()
    {
        private readonly TState toState;
        private readonly ITransitionDelegate<TState> transitionDelegate;

        public EventFireMethod EventFireMethod { get; }
        public int EventFireMethodInt => (int)this.EventFireMethod;
        public IEvent Event { get; }
        public object EventData { get; }

        public ForcedTransitionInvoker(TState toState, IEvent @event, object eventData, ITransitionDelegate<TState> transitionDelegate)
        {
            this.toState = toState;
            // This is never actually references, but needs to be part of ITransitionInvoker
            this.EventFireMethod = EventFireMethod.Fire;
            this.Event = @event;
            this.EventData = eventData;
            this.transitionDelegate = transitionDelegate;
        }

        public bool TryInvoke(TState sourceState)
        {
            if (this.toState.ParentStateMachine.CurrentState != this.toState)
            {
                var transitionInfo = new ForcedTransitionInfo<TState>(this.toState.ParentStateMachine.CurrentState, this.toState, this.Event, this.EventData, this.EventFireMethod);
                this.transitionDelegate.CoordinateTransition(transitionInfo, null);
            }

            return true;
        }
    }
}
