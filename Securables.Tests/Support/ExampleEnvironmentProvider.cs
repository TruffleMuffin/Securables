﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Securables.Contracts;

namespace Securables.Tests.Support
{
    class ExampleEnvironmentProvider : IEnvironmentProvider
    {
        private readonly Dictionary<string, object> environments = new Dictionary<string, object>
            {
                { "CurrentUser", new CurrentUserEnvironment { UserId = new Guid("880A00AD-5C40-447B-821A-2679E757B267") } }, 
                { "Acl", new AclEnvironment { Entries = new List<Acl>{ new Acl { Allow = false } } } },
                { "LongRunning", new AclEnvironment { Entries = new List<Acl>{ new Acl { Allow = true } } } }
            };

        public CacheOptions Cache { get { return new CacheOptions { Cacheable = true, Period = TimeSpan.FromSeconds(10) }; } }
        public string[] SupportedKeys { get { return environments.Keys.ToArray(); } }

        public string Component { get { return "Example"; } }

        public async Task<dynamic> GetAsync(string key, DecisionContext context)
        {
            if (key == "LongRunning")
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }

            return await Task.FromResult(environments[key]);
        }
    }
}