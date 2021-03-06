﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Example.Support;
using Decisions.Exceptions;
using Decisions.Services;
using MbUnit.Framework;

namespace Decisions.Tests.Application.Services
{
    [TestFixture]
    class DecisionsServiceTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private DecisionService target;

        [SetUp]
        void SetUp()
        {
            environmentService = new EnvironmentService(new[] { new ExampleEnvironmentProvider() });
            var policies = new Dictionary<string, IPolicy>
                {
                    { "A", new AlphaPolicy() }, 
                    { "B", new BetaPolicy() }, 
                    { "C", new AlphaPolicy() }, 
                    { "D", new AlphaPolicy() }, 
                    { "E", new CappaPolicy { MatchUserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267")} }, 
                    { "F", new CappaPolicy { MatchUserId = new Guid("1E9A7C0C-FC86-4516-BA42-F7232E65A12C")} }, 
                    { "G", new DeltaPolicy() }, 
                    { "H", new LongDeltaPolicy() }
                };
            policyService = new PolicyService(new[] { new PolicyProvider(policies) }, environmentService);
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
        }

        [AsyncTest]
        [Row("A", true)]
        [Row("A1", false)]
        [Row("B", false)]
        [Row("C", true)]
        [Row("C1", false)]
        [Row("D", false)]
        [Row("E", true)]
        [Row("F", false)]
        [Row("G", false)]
        [Row("H", true)]
        [Row("J", true)]
        [Row("K", true)]
        [Row("K1", true)]
        async Task CheckAsync_Decision_Expected(string alias, bool expected)
        {
            var result = await target.CheckAsync(DecisionContext.Create().Using("Example").As("trufflemuffin").Has(alias).On(new { id = 1 }));
            Assert.AreEqual(expected, result);
        }
        
        [AsyncTest]
        [Row("H", true, 4)]
        [Row("I", true, 1)]
        async Task CheckAsync_Decision_Expected_TimeConstraint(string alias, bool expected, int seconds)
        {
            DateTime start = DateTime.Now;

            var result = await target.CheckAsync(DecisionContext.Create().Using("Example").As("trufflemuffin").Has(alias).On(new { id = 1 }));

            DateTime end = DateTime.Now;

            Assert.AreEqual(expected, result);
            Assert.AreApproximatelyEqual(end, start, TimeSpan.FromSeconds(seconds));
        }

        [Test]
        [ExpectedException(typeof(ConfigurationMalformedException))]
        void Construction_WithMissingNameSpace_UsefulException()
        {
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MissingNamespace.config"), policyService);
        }

        [Test]
        [ExpectedException(typeof(ConfigurationMalformedException))]
        void Construction_WithBadNameSpace_UsefulException()
        {
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BadNamespace.config"), policyService);
        }

        [Test]
        [ExpectedException(typeof(ConfigurationMalformedException))]
        void Construction_WithMissingDecisions_UsefulException()
        {
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MissingDecisions.config"), policyService);
        }

        [Test]
        [ExpectedException(typeof(ConfigurationMalformedException))]
        void Construction_WithMissingItem_UsefulException()
        {
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MissingItem.config"), policyService);
        }

        [Test]
        [ExpectedException(typeof(ConfigurationMalformedException))]
        void Construction_WithBadItem_UsefulException()
        {
            target = new DecisionService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BadItem.config"), policyService);
        }
    }
}

