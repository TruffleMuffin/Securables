﻿using Securables.Contracts;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Securables.Application.Services
{
    /// <summary>
    /// A service for retrieving policies regarding a <see cref="DecisionContext"/> for use in determining its <see cref="Decision"/> via the <see cref="ISecurablesService"/>.
    /// </summary>
    internal class PolicyService
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, AbstractPolicy>> policies = new ConcurrentDictionary<string, ConcurrentDictionary<string, AbstractPolicy>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public PolicyService(IEnumerable<IPolicyProvider> providers)
        {
            if (providers == null || providers.Any() == false) throw new ArgumentException("Some providers are required to initialize the Securables.Application.Services.PolicyService");

            foreach (var policyProvider in providers)
            {
                var componentPolicies = new ConcurrentDictionary<string, AbstractPolicy>(policyProvider.GetPolicies().ToList());
                policies.AddOrUpdate(policyProvider.Component, componentPolicies, (key, oldValue) => componentPolicies);
            }
        }

        /// <summary>
        /// Gets the <see cref="AbstractPolicy" /> with the specified key.
        /// </summary>
        /// <param name="component">The component the policy will be found in.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// An environment, likely an instance of a class from an external assembly.
        /// </returns>
        public AbstractPolicy Get(string component, string key)
        {
            return policies[component][key];
        }
    }
}