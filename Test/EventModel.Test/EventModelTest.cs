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
        private IServiceProvider services;
        private WeaponSystem weapon;
        private GameUnitFireAllocation weaponAllocation;
        private GamePhaseEvent phaseEvent;
        private WeaponAttackEvent attackEvent;
        private TestDummyActor totalDummy;
        private TestDummyPhaseActor phaseDummy;

        private EventHandlingEngine engine;

        [TestInitialize]
        public void InitServices()
        {
            this.services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IDiceUtility, MockDice>() // Always rolls sixes on d6; always rolls 99 on d100
                .BuildServiceProvider();

            this.weapon = new BeamBatterySystem(3, "(All arcs)");
            this.weaponAllocation = new GameUnitFireAllocation();

            this.phaseEvent = new FiringPhaseEvent(1, 1) as GamePhaseEvent;
            this.attackEvent = new WeaponAttackEvent(new TargetingData(), new AttackData(this.weapon, this.weaponAllocation));

            this.totalDummy = new TestDummyActor(this.services);
            this.phaseDummy = new TestDummyPhaseActor(this.services);

            this.engine = new EventHandlingEngine();
        }

        [TestMethod]
        public void TestDummyActors()
        {
            this.totalDummy.ReceiveEvent(this.phaseEvent);

            // Base class received event
            Assert.AreEqual(1, this.totalDummy.EventReceivedCount);

            // ... but TestDummyActor doesn't have an override for FiringPhaseEvent
            Assert.AreEqual(0, this.totalDummy.TestDummyActorEventReceivedCount);

            this.phaseDummy.ReceiveEvent(this.phaseEvent);
            Assert.AreEqual(0, this.phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(1, this.phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, this.phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(1, this.phaseDummy.EventReceivedCount);

            this.totalDummy.ReceiveEvent(this.attackEvent);
            Assert.AreEqual(0, this.totalDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, this.totalDummy.EventReceivedCount);

            this.phaseDummy.ReceiveEvent(this.attackEvent);
            Assert.AreEqual(1, this.phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(1, this.phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, this.phaseDummy.TestDummyActorEventReceivedCount);
            Assert.AreEqual(2, this.phaseDummy.EventReceivedCount);
        }

        [TestMethod]
        public void TestDummyActorEventHandlerTypeRouting()
        {
            // Processes pretty much nothing
            var totalDummy = new TestDummyActor(this.services);

            // Processes GamePhase, FiringPhase, and WeaponAttack events
            var phaseDummy = new TestDummyPhaseActor(this.services);

            var actors = new List<IEventActor>()
            {
                totalDummy,
                phaseDummy
            };

            this.engine.ExecuteGamePhase(actors, this.phaseEvent, 1, 1);
            Assert.AreEqual(1, phaseDummy.EventReceivedCount);
            Assert.AreEqual(1, phaseDummy.GamePhaseEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.WeaponAttackEventDetectedCount);
            Assert.AreEqual(0, phaseDummy.TestDummyActorEventReceivedCount);

            Assert.AreEqual(1, totalDummy.EventReceivedCount);
            Assert.AreEqual(0, totalDummy.TestDummyActorEventReceivedCount);
        }
    }
}
