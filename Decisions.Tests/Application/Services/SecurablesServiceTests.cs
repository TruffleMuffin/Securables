﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Decisions.Application.Services;
using Decisions.Contracts;
using Decisions.Contracts.Providers;
using Decisions.Tests.Support;
using MbUnit.Framework;

namespace Decisions.Tests.Application.Services
{
    [TestFixture]
    class DecisionsServiceTests
    {
        private PolicyService policyService;
        private EnvironmentService environmentService;
        private DecisionsService target;

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
            target = new DecisionsService(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test-Decisions.config"), policyService);
        }

        [AsyncTest]
        [Row("A", true)]
        [Row("B", false)]
        [Row("C", true)]
        [Row("D", false)]
        [Row("E", true)]
        [Row("F", false)]
        [Row("G", false)]
        [Row("H", true)]
        async Task CheckAsync_Decision_Expected(string alias, bool expected)
        {
            var result = await target.CheckAsync(DecisionContext.Create().For("Example").As("gareth").On(alias).Against("id", 1));
            Assert.AreEqual(expected, result);
        }
        
        [AsyncTest]
        [Row("H", true, 4)]
        [Row("I", true, 1)]
        async Task CheckAsync_Decision_Expected_TimeConstraint(string alias, bool expected, int seconds)
        {
            DateTime start = DateTime.Now;

            var result = await target.CheckAsync(DecisionContext.Create().For("Example").As("gareth").On(alias).Against("id", 1));

            DateTime end = DateTime.Now;

            Assert.AreEqual(expected, result);
            Assert.AreApproximatelyEqual(end, start, TimeSpan.FromSeconds(seconds));
        }
    }
}

