// <copyright file="EventModelTest.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace EventModel.Test
{
    using System.Collections.Generic;
    using FireAndManeuver.EventModel;
    using FireAndManeuver.EventModel.EventActors;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventModelTest
    {
        [TestMethod]
        public void TestDummyActors()
        {
            var phaseEvent = new FiringPhaseEvent(1, 1) as GamePhaseEvent;
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var totalDummy = new TestDummyActor();
            var phaseDummy = new TestDummyPhaseActor();

            totalDummy.ReceiveEvent(phaseEvent);

            // Base class received event
            Assert.AreEqual(1, totalDummy.EventReceivedCount);

            // ... but TestDummyActor doesn't have an override for FiringPhaseEvent
            Assert.AreEqual(0, totalDummy.TestDummyActorEventReceivedCount);

            phaseDummy.ReceiveEvent(phaseEvent);
            Assert.AreEqual(0, phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(1, phaseDummy.EventReceivedCount);

            totalDummy.ReceiveEvent(attackEvent);
            Assert.AreEqual(0, totalDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, totalDummy.EventReceivedCount);

            phaseDummy.ReceiveEvent(attackEvent);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(1, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, phaseDummy.EventReceivedCount);
        }

        [TestMethod]
        public void TestDummyActorEventHandlerTypeRouting()
        {
            var phaseEvent = new FiringPhaseEvent(1, 1);
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var engine = new EventHandlingEngine();

            var totalDummy = new TestDummyActor();
            var phaseDummy = new TestDummyPhaseActor();

            var actors = new List<IEventActor>()
            {
                totalDummy,
                phaseDummy
            };

            engine.ExecuteGamePhase(actors, phaseEvent, 1, 1);
            Assert.AreEqual(1, phaseDummy.EventReceivedCount);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.TestDummyActorEventReceivedCount);

            Assert.AreEqual(1, totalDummy.EventReceivedCount);
            Assert.AreEqual(0, totalDummy.TestDummyActorEventReceivedCount);
        }
    }
}
