using Consul.Contracts.Client;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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

    /// <summary>
    /// Represents a persistant connection to a Consul agent. Instances of this class should be created rarely and reused often.
    /// </summary>
    public partial class ConsulClient : IDisposable
    {

        /// <summary>
        /// This class is used to group all the configurable bits of a ConsulClient into a single pointer reference
        /// which is great for implementing reconfiguration later.
        /// </summary>
        private class ConsulClientConfigurationContainer
        {

            internal readonly bool skipClientDispose;
            internal readonly HttpClient HttpClient;
#if CORECLR
            internal readonly HttpClientHandler HttpHandler;
#else
            internal readonly WebRequestHandler HttpHandler;
#endif
            public readonly ConsulClientConfiguration Config;

            public ConsulClientConfigurationContainer()
            {
                Config = new ConsulClientConfiguration();
#if CORECLR
                HttpHandler = new HttpClientHandler();
#else
                HttpHandler = new WebRequestHandler();
#endif
                HttpClient = new HttpClient(HttpHandler);
                HttpClient.Timeout = TimeSpan.FromMinutes(15);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }

            #region Old style config handling
            public ConsulClientConfigurationContainer(ConsulClientConfiguration config, HttpClient client)
            {
                skipClientDispose = true;
                Config = config;
                HttpClient = client;
            }

            public ConsulClientConfigurationContainer(ConsulClientConfiguration config)
            {
                Config = config;
#if CORECLR
                HttpHandler = new HttpClientHandler();
#else
                HttpHandler = new WebRequestHandler();
#endif
                HttpClient = new HttpClient(HttpHandler);
                HttpClient.Timeout = TimeSpan.FromMinutes(15);
                HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpClient.DefaultRequestHeaders.Add("Keep-Alive", "true");
            }
            #endregion

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if (HttpClient != null && !skipClientDispose)
                        {
                            HttpClient.Dispose();
                        }
                        if (HttpHandler != null)
                        {
                            HttpHandler.Dispose();
                        }
                    }

                    disposedValue = true;
                }
            }

            //~ConsulClient()
            //{
            //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //    Dispose(false);
            //}

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public void CheckDisposed()
            {
                if (disposedValue)
                {
                    throw new ObjectDisposedException(typeof(ConsulClientConfigurationContainer).FullName.ToString());
                }
            }
            #endregion
        }

        private ConsulClientConfigurationContainer ConfigContainer;

        internal HttpClient HttpClient { get { return ConfigContainer.HttpClient; } }
#if CORECLR
        internal HttpClientHandler HttpHandler { get { return ConfigContainer.HttpHandler; } }
#else
        internal WebRequestHandler HttpHandler { get { return ConfigContainer.HttpHandler; } }
#endif
        public ConsulClientConfiguration Config { get { return ConfigContainer.Config; } }

        internal readonly JsonSerializer serializer = new JsonSerializer();

        #region New style config with Actions
        /// <summary>
        /// Initializes a new Consul client with a default configuration that connects to 127.0.0.1:8500.
        /// </summary>
        public ConsulClient() : this(null, null, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride) : this(configOverride, null, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration and access the underlying HttpClient for modification.
        /// The HttpClient is modified to set options like the request timeout and headers.
        /// The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride) : this(configOverride, clientOverride, null) { }

        /// <summary>
        /// Initializes a new Consul client with the ability to set portions of the configuration and access the underlying HttpClient and WebRequestHandler for modification.
        /// The HttpClient is modified to set options like the request timeout and headers.
        /// The WebRequestHandler is modified to set options like Proxy and Credentials.
        /// The Timeout property also applies to all long-poll requests and should be set to a value that will encompass all successful requests.
        /// </summary>
        /// <param name="configOverride">The Action to modify the default configuration with</param>
        /// <param name="clientOverride">The Action to modify the HttpClient with</param>
        /// <param name="handlerOverride">The Action to modify the WebRequestHandler with</param>
#if !CORECLR
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride, Action<WebRequestHandler> handlerOverride)
#else
        public ConsulClient(Action<ConsulClientConfiguration> configOverride, Action<HttpClient> clientOverride, Action<HttpClientHandler> handlerOverride)
#endif
        {
            var ctr = new ConsulClientConfigurationContainer();

            configOverride?.Invoke(ctr.Config);
            ApplyConfig(ctr.Config, ctr.HttpHandler, ctr.HttpClient);
            handlerOverride?.Invoke(ctr.HttpHandler);
            clientOverride?.Invoke(ctr.HttpClient);

            ConfigContainer = ctr;

            InitializeEndpoints();
        }
        #endregion

        #region Old style config
        /// <summary>
        /// Initializes a new Consul client with the configuration specified.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        [Obsolete("This constructor is no longer necessary due to the new Action based constructors and will be removed when 0.8.0 is released." +
            "Please use the ConsulClient(Action<ConsulClientConfiguration>) constructor to set configuration options.", false)]
        public ConsulClient(ConsulClientConfiguration config)
        {
            config.Updated += HandleConfigUpdateEvent;
            var ctr = new ConsulClientConfigurationContainer(config);
            ApplyConfig(ctr.Config, ctr.HttpHandler, ctr.HttpClient);

            ConfigContainer = ctr;
            InitializeEndpoints();
        }

        /// <summary>
        /// Initializes a new Consul client with the configuration specified and a custom HttpClient, which is useful for setting proxies/custom timeouts.
        /// The HttpClient must accept the "application/json" content type and the Timeout property should be set to at least 15 minutes to allow for blocking queries.
        /// </summary>
        /// <param name="config">A Consul client configuration</param>
        /// <param name="client">A custom HttpClient</param>
        [Obsolete("This constructor is no longer necessary due to the new Action based constructors and will be removed when 0.8.0 is released." +
            "Please use one of the ConsulClient(Action<>) constructors instead to set internal options on the HttpClient/WebRequestHandler.", false)]
        public ConsulClient(ConsulClientConfiguration config, HttpClient client)
        {
            var ctr = new ConsulClientConfigurationContainer(config, client);
            if (!ctr.HttpClient.DefaultRequestHeaders.Accept.Contains(new MediaTypeWithQualityHeaderValue("application/json")))
            {
                throw new ArgumentException("HttpClient must accept the application/json content type", nameof(client));
            }
            ConfigContainer = ctr;
            InitializeEndpoints();
        }
        #endregion

        private void InitializeEndpoints()
        {
            _acl = new Lazy<ACL>(() => new ACL(this));
            _agent = new Lazy<Agent>(() => new Agent(this));
            _catalog = new Lazy<Catalog>(() => new Catalog(this));
            _coordinate = new Lazy<Coordinate>(() => new Coordinate(this));
            _event = new Lazy<Event>(() => new Event(this));
            _health = new Lazy<Health>(() => new Health(this));
            _kv = new Lazy<KV>(() => new KV(this));
            _operator = new Lazy<Operator>(() => new Operator(this));
            _preparedquery = new Lazy<PreparedQuery>(() => new PreparedQuery(this));
            _raw = new Lazy<Raw>(() => new Raw(this));
            _session = new Lazy<Session>(() => new Session(this));
            _snapshot = new Lazy<Snapshot>(() => new Snapshot(this));
            _status = new Lazy<Status>(() => new Status(this));
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Config.Updated -= HandleConfigUpdateEvent;
                    if (ConfigContainer != null)
                    {
                        ConfigContainer.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        //~ConsulClient()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void CheckDisposed()
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(typeof(ConsulClient).FullName.ToString());
            }
        }
        #endregion

        void HandleConfigUpdateEvent(object sender, EventArgs e)
        {
            ApplyConfig(sender as ConsulClientConfiguration, HttpHandler, HttpClient);

        }
#if !CORECLR
        void ApplyConfig(ConsulClientConfiguration config, WebRequestHandler handler, HttpClient client)
#else
        void ApplyConfig(ConsulClientConfiguration config, HttpClientHandler handler, HttpClient client)
#endif        
        {
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.HttpAuth != null)
#pragma warning restore CS0618 // Type or member is obsolete
            {
#pragma warning disable CS0618 // Type or member is obsolete
                handler.Credentials = config.HttpAuth;
#pragma warning restore CS0618 // Type or member is obsolete
            }
#if !__MonoCS__
            if (config.ClientCertificateSupported)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                if (config.ClientCertificate != null)
#pragma warning restore CS0618 // Type or member is obsolete
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
#pragma warning disable CS0618 // Type or member is obsolete
                    handler.ClientCertificates.Add(config.ClientCertificate);
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ClientCertificates.Clear();
                }
            }
#endif
#if !CORECLR
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.DisableServerCertificateValidation)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                handler.ServerCertificateValidationCallback += (certSender, cert, chain, sslPolicyErrors) => { return true; };
            }
            else
            {
                handler.ServerCertificateValidationCallback = null;
            }
#else
#pragma warning disable CS0618 // Type or member is obsolete
            if (config.DisableServerCertificateValidation)
#pragma warning restore CS0618 // Type or member is obsolete
            {
                handler.ServerCertificateCustomValidationCallback += (certSender, cert, chain, sslPolicyErrors) => { return true; };
            }
            else
            {
                handler.ServerCertificateCustomValidationCallback = null;
            }
#endif

            if (!string.IsNullOrEmpty(config.Token))
            {
                if (client.DefaultRequestHeaders.Contains("X-Consul-Token"))
                {
                    client.DefaultRequestHeaders.Remove("X-Consul-Token");
                }
                client.DefaultRequestHeaders.Add("X-Consul-Token", config.Token);
            }
        }

        internal GetRequest<TOut> Get<TOut>(string path, QueryOptions opts = null)
        {
            return new GetRequest<TOut>(this, path, opts ?? QueryOptions.Default);
        }

        internal DeleteReturnRequest<TOut> DeleteReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new DeleteReturnRequest<TOut>(this, path, opts ?? WriteOptions.Default);
        }

        internal DeleteRequest Delete(string path, WriteOptions opts = null)
        {
            return new DeleteRequest(this, path, opts ?? WriteOptions.Default);
        }

        internal DeleteAcceptingRequest<TIn> DeleteAccepting<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new DeleteAcceptingRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PutReturningRequest<TOut> PutReturning<TOut>(string path, WriteOptions opts = null)
        {
            return new PutReturningRequest<TOut>(this, path, opts ?? WriteOptions.Default);
        }

        internal PutRequest<TIn> Put<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new PutRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PutNothingRequest PutNothing(string path, WriteOptions opts = null)
        {
            return new PutNothingRequest(this, path, opts ?? WriteOptions.Default);
        }

        internal PutRequest<TIn, TOut> Put<TIn, TOut>(string path, TIn body, WriteOptions opts = null)
        {
            return new PutRequest<TIn, TOut>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PostRequest<TIn> Post<TIn>(string path, TIn body, WriteOptions opts = null)
        {
            return new PostRequest<TIn>(this, path, body, opts ?? WriteOptions.Default);
        }

        internal PostRequest<TIn, TOut> Post<TIn, TOut>(string path, TIn body, WriteOptions opts = null)
        {
            return new PostRequest<TIn, TOut>(this, path, body, opts ?? WriteOptions.Default);
        }
    }
}