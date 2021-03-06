﻿using System.Collections.Generic;

namespace Decisions.Contracts.Providers
{
    /// <summary>
    /// A simple implementation of the <see cref="IPolicyProvider"/> interface, which should suit
    /// the needs of most clients.
    /// </summary>
    public class PolicyProvider : IPolicyProvider
    {
        private readonly IDictionary<string, IPolicy> policies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyProvider" /> class.
        /// </summary>
        /// <param name="policies">The provided policies.</param>
        public PolicyProvider(IDictionary<string, IPolicy> policies)
        {
            this.policies = policies;
        }

        /// <summary>
        /// Gets all the policies this instance of the provider wishes to offer.
        /// </summary>
        /// <returns>
        /// A dictionary of policies, the key should be a identifier that is used to link to the decision.
        /// </returns>
        public IDictionary<string, IPolicy> GetPolicies()
        {
            return policies;
        }
    }
}
