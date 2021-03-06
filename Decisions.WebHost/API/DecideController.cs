﻿using System.Threading.Tasks;
using System.Web.Http;
using Decisions.Contracts;

namespace Decisions.WebHost.API
{
    /// <summary>
    /// Describes the Decide endpoint for the Decisions API.
    /// </summary>
    public class DecideController : ApiController
    {
        private readonly IDecisionService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="DecideController"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        public DecideController(IDecisionService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Handles a Decision request.
        /// HTTP Verb: GET
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        /// <param name="sourceId">The identifier of the source of the request.</param>
        /// <param name="roleName">Name of the role the user claims to have on the entity within the namespace.</param>
        /// <param name="targetId">The identifier of the target of the request.</param>
        /// <returns>
        /// A Decision indicating the result of the query.
        /// </returns>
        [Route("Api/Decide/{namespace}/{sourceId}/{roleName}/{targetId?}")]
        public async Task<bool> Get(string @namespace, string sourceId, string roleName, string targetId = null)
        {
            return await service.CheckAsync(Resolve(@namespace, sourceId, roleName, targetId));
        }

        /// <summary>
        /// Resolves the <see cref="DecisionContext"/> based on the provided parameters.
        /// </summary>
        /// <param name="namespace">The namespace.</param>
        /// <param name="sourceId">The source id.</param>
        /// <param name="roleName">Name of the role.</param>
        /// <param name="targetId">The target id.</param>
        /// <returns>A <see cref="DecisionContext"/></returns>
        private static DecisionContext Resolve(string @namespace, string sourceId, string roleName, string targetId)
        {
            return DecisionContext.Create().Using(@namespace).As(sourceId).Has(roleName).On(targetId);
        }
    }
}