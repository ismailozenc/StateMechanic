using System;

namespace StateMechanic
{
    /// <summary>
    /// A state machine, which may exist as a child state machine
    /// </summary>
    public abstract class ChildStateMachine<TState> : IStateMachine, IStateDelegate<TState>
        where TState : StateBase<TState>, new()
    {
        /// <summary>
        /// Gets the initial state of this state machine
        /// </summary>
        public TState InitialState { get; private set; }

        private TState _currentState;

        /// <summary>
        /// Gets the state which this state machine is currently in
        /// </summary>
        public TState CurrentState
        {
            get
            {
                this.EnsureNoFault();
                return this._currentState;
            }
            private set
            {
                this._currentState = value;
            }
        }

        /// <summary>
        /// Gets the name given to this state machine when it was created
        /// </summary>
        public string Name { get; }

        IState IStateMachine.CurrentState => this.CurrentState;
        IState IStateMachine.InitialState => this.InitialState;

        internal ChildStateMachine(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// Create a new state and add it to this state machine
        /// </summary>
        /// <param name="name">Name given to the state</param>
        /// <returns>The new state</returns>
        public TState CreateState(string name = null)
        {
            return this.CreateState<TState>(name);
        }

        /// <summary>
        /// Create a new state (with a custom type) and add it to this state machine
        /// </summary>
        /// <param name="name">Name given to the state</param>
        /// <returns>The new state</returns>
        public TNewState CreateState<TNewState>(string name = null) where TNewState : TState, new()
        {
            var state = new TNewState();
            state.Initialize(name, this as StateMachine<TState>);
            return state;
        }

        /// <summary>
        /// Create the state which this state machine will be in when it first starts. This must be called exactly once per state machine
        /// </summary>
        /// <param name="name">Name given to the state</param>
        /// <returns>The new state</returns>
        public TState CreateInitialState(string name = null)
        {
            return this.CreateInitialState<TState>(name);
        }

        /// <summary>
        /// Create the state (with a custom type) which this state machine will be in when it first starts. This must be called exactly once per state machine
        /// </summary>
        /// <param name="name">Name given to the state</param>
        /// <returns>The new state</returns>
        public TNewState CreateInitialState<TNewState>(string name = null) where TNewState : TState, new()
        {
            var state = this.CreateState<TNewState>(name);
            this.SetInitialState(state);
            return state;
        }

        private void SetInitialState(TState state)
        {
            if (this.InitialState != null)
                throw new InvalidOperationException("Initial state has already been set");

            this.InitialState = state;

            // Child state machines start off in no state, and progress to the initial state
            // Normal state machines start in the start state
            // The exception is child state machines which are children of their parent's initial state, where the parent is not a child state machine

            this.ResetCurrentState();
        }

        internal void ResetCurrentState()
        {
            this.CurrentState = this.InitialState;
        }

        // This would be protected *and* internal if that were possible
        internal virtual void HandleTransitionNotFound(IEvent @event, EventFireMethod eventFireMethod)
        {
            // Overridden in StateMachine to do things
        }

        internal void SetCurrentState(TState state)
        {
            this.EnsureSuitableForUse();

            // This should only be possible because of an internal error
            Trace.Assert(state == null || state.ParentStateMachine == this, $"Cannot set current state of {this} to {state}, as that state does not belong to that state machine");

            this.CurrentState = state;
        }

        // I'd make this protected *and* internal if I could
        internal bool RequestEventFire<TTransitionInvoker>(TTransitionInvoker transitionInvoker)
            where TTransitionInvoker : ITransitionInvoker<TState>
        {
            this.EnsureCurrentStateSuitableForTransition();

            bool success;

            // No? Invoke it on ourselves
            success = transitionInvoker.TryInvoke(this.CurrentState);

            if (!success)
                this.HandleTransitionNotFound(transitionInvoker.Event, (EventFireMethod)transitionInvoker.EventFireMethodInt);

            return success;
        }

        private void EnsureSuitableForUse()
        {
            if (this.InitialState == null)
                throw new InvalidOperationException($"Initial state on {this.ToString()} not yet set. You must call CreateInitialState");
        }

        private void EnsureCurrentStateSuitableForTransition()
        {
            this.EnsureSuitableForUse();

            // This should only be possible because of an internal error
            Trace.Assert(this.CurrentState != null, "Child state machine's parent state is not current. This state machine is currently disabled");
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        [ExcludeFromCoverage]
        public override string ToString()
        {
            var stateName = (this.CurrentState == null) ? "None" : (this.CurrentState.Name ?? "(unnamed)");
            return $"<StateMachine Name={this.Name ?? "(unnamed)"} State={stateName}>";
        }

        /// <summary>
        /// Ensures the state machine is not faulty.
        /// </summary>
        public abstract void EnsureNoFault();
    }
}
