using System;
using Consul.Interfaces;

namespace Consul
{
    public partial class ConsulClient : IConsulClient
    {
        private Lazy<ACL> _acl;

        /// <summary>
        /// ACL returns a handle to the ACL endpoints
        /// </summary>
        public IACLEndpoint ACL
        {
            get { return _acl.Value; }
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Agent> _agent;

        /// <summary>
        /// Agent returns a handle to the agent endpoints
        /// </summary>
        public IAgentEndpoint Agent
        {
            get { return _agent.Value; }
        }
    }

    public partial class ConsulClient : IConsulClient
    {
        private Lazy<Catalog> _catalog;

        /// <summary>
        /// Catalog returns a handle to the catalog endpoints
        /// </summary>
        public ICatalogEndpoint Catalog
        {
            get { return _catalog.Value; }
        }
    }
}