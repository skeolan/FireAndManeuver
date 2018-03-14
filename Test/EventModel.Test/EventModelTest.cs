// <copyright file="EventModelTest.cs" company="Patrick Maughan">
// Copyright (c) Patrick Maughan. All rights reserved.
// </copyright>

namespace FireAndManeuver.EventModel.Test
{
    using System;
    using System.Collections.Generic;
    using FireAndManeuver.EventModel;
    using FireAndManeuver.EventModel.EventActors;
    using FireAndManeuver.GameModel;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EventModelTest
    {
        public IServiceProvider InitServices()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IDiceUtility, MockDice>() // Always rolls sixes on d6; always rolls 99 on d100
                .BuildServiceProvider();

            return services;
        }

        [TestMethod]
        public void TestDummyActors()
        {
            var services = this.InitServices();

            var phaseEvent = new FiringPhaseEvent(1, 1) as GamePhaseEvent;
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var totalDummy = new TestDummyActor(services);
            var phaseDummy = new TestDummyPhaseActor(services);

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
            var services = this.InitServices();

            var phaseEvent = new FiringPhaseEvent(1, 1);
            var attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData());

            var engine = new EventHandlingEngine();

            // Processes pretty much nothing
            var totalDummy = new TestDummyActor(services);

            // Processes GamePhase, FiringPhase, and WeaponAttack events
            var phaseDummy = new TestDummyPhaseActor(services);

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