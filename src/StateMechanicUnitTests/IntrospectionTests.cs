using NUnit.Framework;
using StateMechanic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMechanicUnitTests
{
    [TestFixture]
    public class IntrospectionTests
    {
        private class StateData
        {
        }
        private class EventData
        {
        }

        [Test]
        public void InitialStateAddedToParentStateMachine()
        {
            var stateMachine = new StateMachine("State Machine");
            var state = stateMachine.CreateInitialState("State");
            Assert.AreEqual(state, stateMachine.InitialState);
            Assert.That(stateMachine.CurrentState, Is.EqualTo(state));
        }

        [Test]
        public void StateReferencesParentStateMachine()
        {
            var stateMachine = new StateMachine("State Machine");
            var state = stateMachine.CreateState("State");
            Assert.AreEqual(stateMachine, state.ParentStateMachine);
            Assert.AreEqual(stateMachine, ((IState)state).ParentStateMachine);
        }

        [Test]
        public void TransitionAddedToState()
        {
            var stateMachine = new StateMachine("State Machine");
            var state1 = stateMachine.CreateState("State 1");
            var state2 = stateMachine.CreateState("State 2");
            var evt = new Event("Event");
            state1.TransitionOn(evt).To(state2);

            Assert.AreEqual(1, state1.Transitions.Count);
            Assert.AreEqual(state1, state1.Transitions[0].From);
            Assert.AreEqual(state2, state1.Transitions[0].To);
            Assert.AreEqual(evt, state1.Transitions[0].Event);
            Assert.False(state1.Transitions[0].IsInnerTransition);
        }

        [Test]
        public void TransitionTAddedToState()
        {
            var stateMachine = new StateMachine("State Machine");
            var state1 = stateMachine.CreateState("State 1");
            var state2 = stateMachine.CreateState("State 2");
            var evt = new Event<EventData>("Event");
            state1.TransitionOn(evt).To(state2);

            Assert.AreEqual(1, state1.Transitions.Count);
            Assert.AreEqual(state1, state1.Transitions[0].From);
            Assert.AreEqual(state2, state1.Transitions[0].To);
            Assert.AreEqual(evt, state1.Transitions[0].Event);
            Assert.False(state1.Transitions[0].IsInnerTransition);
        }

        [Test]
        public void InnerTransitionAddedToState()
        {
            var stateMachine = new StateMachine("State Machine");
            var state1 = stateMachine.CreateState("State 1");
            var evt = new Event("Event");
            state1.InnerSelfTransitionOn(evt);

            Assert.AreEqual(1, state1.Transitions.Count);
            Assert.AreEqual(state1, state1.Transitions[0].From);
            Assert.AreEqual(state1, state1.Transitions[0].To);
            Assert.AreEqual(evt, state1.Transitions[0].Event);
            Assert.True(state1.Transitions[0].IsInnerTransition);
        }

        [Test]
        public void TransitionReportsCorrectInfo()
        {
            Action<TransitionInfo<State>> handler = i => { };
            Func<TransitionInfo<State>, bool> guard = i => true;
            var sm = new StateMachine();
            var state1 = sm.CreateInitialState("state1");
            var state2 = sm.CreateState("state2");
            var evt = new Event();
            var transition = state1.TransitionOn(evt).To(state2).WithHandler(handler).WithGuard(guard);

            Assert.AreEqual(state1, transition.From);
            Assert.AreEqual(state2, transition.To);
            Assert.AreEqual(evt, transition.Event);
            Assert.AreEqual(evt, ((ITransition<State>)transition).Event);
            Assert.False(transition.IsInnerTransition);
            Assert.False(((ITransition<State>)transition).IsDynamicTransition);
            Assert.AreEqual(handler, transition.Handler);
            Assert.True(transition.HasGuard);
            Assert.AreEqual(guard, transition.Guard);
        }

        [Test]
        public void TransitionWithEventDataReportsCorrectInfo()
        {
            Action<TransitionInfo<State, string>> handler = i => { };
            Func<TransitionInfo<State, string>, bool> guard = i => true;
            var sm = new StateMachine();
            var state1 = sm.CreateInitialState("state1");
            var state2 = sm.CreateState("state2");
            var evt = new Event<string>();
            var transition = state1.TransitionOn(evt).To(state2).WithHandler(handler).WithGuard(guard);

            Assert.AreEqual(state1, transition.From);
            Assert.AreEqual(state2, transition.To);
            Assert.AreEqual(evt, transition.Event);
            Assert.AreEqual(evt, ((ITransition<State>)transition).Event);
            Assert.False(transition.IsInnerTransition);
            Assert.False(((ITransition<State>)transition).IsDynamicTransition);
            Assert.AreEqual(handler, transition.Handler);
            Assert.True(transition.HasGuard);
            Assert.AreEqual(guard, transition.Guard);
        }
    }
}
