using System.Collections.Generic;

namespace StateMechanic
{
    /// <summary>
    /// A state machine, which may exist as a child state machine
    /// </summary>
    public interface IStateMachine
    {
        /// <summary>
        /// Gets the name given to this state machine when it was created
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the state which this state machine is currently in
        /// </summary>
        IState CurrentState { get; }

        /// <summary>
        /// Gets the initial state of this state machine
        /// </summary>
        IState InitialState { get; }

        /// <summary>
        /// Ensures the state machine is not faulty.
        /// </summary>
        void EnsureNoFault();
    }
}
