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
            var phaseEvent = new FiringPhase();
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var totalDummy = new TestDummyActor();
            var phaseDummy = new TestDummyPhaseActor();

            totalDummy.ProcessEvent(phaseEvent);
            Assert.AreEqual(1, totalDummy.EventReceivedCount);
            Assert.AreEqual(1, totalDummy.TestDummyActorEventReceivedCount);

            phaseDummy.ProcessEvent(phaseEvent);
            Assert.AreEqual(1, phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(1, phaseDummy.EventReceivedCount);

            totalDummy.ProcessEvent(attackEvent);
            Assert.AreEqual(2, totalDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, totalDummy.EventReceivedCount);

            phaseDummy.ProcessEvent(attackEvent);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(1, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(2, phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, phaseDummy.EventReceivedCount);
        }

        [TestMethod]
        public void TestDummyActorEventHandlerTypeRouting()
        {
            var phaseEvent = new FiringPhase();
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
            Assert.AreEqual(1, phaseDummy.TestDummyActorEventReceivedCount);

            Assert.AreEqual(1, totalDummy.EventReceivedCount);
            Assert.AreEqual(1, totalDummy.TestDummyActorEventReceivedCount);
        }
    }
}
