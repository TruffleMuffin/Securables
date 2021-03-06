﻿using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;
using Decisions.Contracts;
using Decisions.Contracts.IoC;
using Decisions.Contracts.Services;
using Decisions.Example.Support;
using Decisions.WebHost;
using MbUnit.Framework;

namespace Decisions.Tests.Contracts.Services
{
    /*
     * ****************************************************** Developer Notice **************************************************************************
     * 
     * These tests will only pass if running Visual Studio in Administrative mode, or the appropriate permissions have been applied to your user account.
     */

    [TestFixture]
    class RemoteDecisionsServiceTests
    {
        private HttpSelfHostServer server;
        private RemoteDecisionService target;

        [SetUp]
        void SetUp()
        {
            Injector.Resolver = new TestResolver();
            var endpoint = "http://localhost:40000";

            target = new RemoteDecisionService(endpoint);
            var config = new HttpSelfHostConfiguration(endpoint);

            config.DependencyResolver = new InjectorDependencyResolver();

            // Decision routes
            config.MapHttpAttributeRoutes();

            server = new HttpSelfHostServer(config);
            server.OpenAsync().Wait();
        }

        [TearDown]
        void TearDown()
        {
            Injector.Resolver = null;
            server.CloseAsync().Wait();
        }

        [AsyncTest]
        async Task Authorized_WithDefaults_AndNoTarget()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("trufflemuffin"), new string[0]);

            var context = DecisionContext.Create().Using("Example").Has("Z");
            var result = await target.CheckAsync(context);
            Assert.IsTrue(result);

            Thread.CurrentPrincipal = null;
        }

        [AsyncTest]
        async Task Authorized_WithDefaults()
        {
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity("trufflemuffin"), new string[0]);

            var context = DecisionContext.Create().Using("Example").Has("A").On(new { id = 1 });
            var result = await target.CheckAsync(context);
            Assert.IsTrue(result);

            Thread.CurrentPrincipal = null;
        }

        [AsyncTest]
        async Task Authorized()
        {
            var context = DecisionContext.Create().Using("Example").As("trufflemuffin").Has("A").On(new { id = 1 });
            var result = await target.CheckAsync(context);
            Assert.IsTrue(result);
        }

        [AsyncTest]
        async Task NotAuthorized()
        {
            var context = DecisionContext.Create().Using("Example").As("trufflemuffin").Has("B").On(new { id = 1 });
            var result = await target.CheckAsync(context);
            Assert.IsFalse(result);
        }
    }
}
