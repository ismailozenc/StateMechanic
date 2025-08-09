using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using StateMechanic;
using System;

namespace StateMechanicUnitTests
{
    [TestFixture]
    public class HandlerOrderTests
    {
        [Test]
        public void CorrectHandlersAreInvokedInNormalTransition()
        {
            var events = new List<string>();
            var sm = new StateMachine("State Machine");
            var evt = new Event("Event");
            var state1 = sm.CreateInitialState("State 1")
                .WithEntry(i => events.Add("State 1 Entry"))
                .WithExit(i => events.Add("State 1 Exit"));
            var state2 = sm.CreateState("State 2")
                .WithEntry(i => events.Add("State 2 Entry"))
                .WithExit(i => events.Add("State 2 Exit"));
            var transition = state1.TransitionOn(evt).To(state2).WithHandler(i => events.Add("Transition 1 2"));

            evt.Fire();

            Assert.That(events, Is.EquivalentTo(new[] { "State 1 Exit", "Transition 1 2", "State 2 Entry" }));
        }

        [Test]
        public void CorrectHandlersAreInvokedInDynamicTransition()
        {
            var events = new List<string>();
            var sm = new StateMachine("State Machine");
            var evt = new Event("Event");
            var state1 = sm.CreateInitialState("State 1").WithExit(i => events.Add("State 1 Exit"));
            var state2 = sm.CreateState("State 2").WithEntry(i => events.Add("State 2 Entry"));
            var transition = state1.TransitionOn(evt).ToDynamic(i =>
            {
                events.Add("Dynamic Handler");
                return state2;
            }).WithHandler(i => events.Add("Transition 1 2"));

            evt.Fire();

            Assert.That(events, Is.EquivalentTo(new[] { "Dynamic Handler", "State 1 Exit", "Transition 1 2", "State 2 Entry" }));
        }

        [Test]
        public void NormalSelfTransitionShouldFireExitAndEntry()
        {
            var events = new List<string>();
            var sm = new StateMachine("State Machine");
            var evt = new Event("Event");
            var state1 = sm.CreateInitialState("State 1").WithEntry(i => events.Add("State 1 Entry")).WithExit(i => events.Add("State 1 Exit"));
            state1.TransitionOn(evt).To(state1).WithHandler(i => events.Add("Transition 1 1"));

            evt.Fire();

            Assert.That(events, Is.EquivalentTo(new[] { "State 1 Exit", "Transition 1 1", "State 1 Entry" }));
        }

        [Test]
        public void InnerSelfTransitionShouldNotFireExitAndEntry()
        {
            var events = new List<string>();
            var sm = new StateMachine("State Machine");
            var evt = new Event("Event");
            var state1 = sm.CreateInitialState("State 1").WithEntry(i => events.Add("State 1 Entry")).WithExit(i => events.Add("State 1 Exit"));
            state1.InnerSelfTransitionOn(evt).WithHandler(i => events.Add("Transition 1 1 Inner"));

            evt.Fire();

            Assert.That(events, Is.EquivalentTo(new[] { "Transition 1 1 Inner" }));
        }

        [Test]
        public void InnerSelfTransitionOnEventTShouldNotFireExitAndEntry()
        {
            var events = new List<string>();
            var sm = new StateMachine("State Machine");
            var evt = new Event<int>("Event");
            var state1 = sm.CreateInitialState("State 1").WithEntry(i => events.Add("State 1 Entry")).WithExit(i => events.Add("State 1 Exit"));
            state1.InnerSelfTransitionOn(evt).WithHandler(i => events.Add("Transition 1 1 Inner"));

            evt.Fire(3);

            Assert.That(events, Is.EquivalentTo(new[] { "Transition 1 1 Inner" }));
        }
    }
}
