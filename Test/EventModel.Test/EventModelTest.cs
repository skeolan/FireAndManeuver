// <copyright file="EventModelTest.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace EventModel.Test
{
    using System.Collections.Generic;
    using FireAndManeuver.EventModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventModelTest
    {
        [TestMethod]
        public void TestDummyActors()
        {
            var phaseEvent = new FiringPhase();
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var totalDummy = new DummyActor();
            var phaseDummy = new DummyPhaseActor();

            totalDummy.ProcessEvent(phaseEvent);
            Assert.AreEqual(1, totalDummy.EventDetectedCount);

            phaseDummy.ProcessEvent(phaseEvent);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.EventDetectedCount);

            totalDummy.ProcessEvent(attackEvent);
            Assert.AreEqual(2, totalDummy.EventDetectedCount);

            phaseDummy.ProcessEvent(attackEvent);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(1, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.EventDetectedCount);
        }

        [TestMethod]
        public void TestDummyActorEventHandlerTypeRouting()
        {
            var phaseEvent = new FiringPhase();
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var engine = new EventHandlingEngine();

            var totalDummy = new DummyActor();
            var phaseDummy = new DummyPhaseActor();

            var actors = new List<IEventActor>()
            {
                totalDummy,
                phaseDummy
            };

            engine.ExecuteGamePhase(actors, phaseEvent, 1, 1);
            Assert.AreEqual(0, phaseDummy.EventDetectedCount);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(1, totalDummy.EventDetectedCount);
        }
    }
}
