using System;

#if !(CORECLR || PORTABLE || PORTABLE40)
using System.Security.Permissions;
using System.Runtime.Serialization;
#endif

namespace Consul.Exceptions
{
    /// <summary>
    /// Represents errors that occur during initalization of the Consul client's configuration.
    /// </summary>
#if !(CORECLR || PORTABLE || PORTABLE40)
    [Serializable]
#endif
    public class ConsulConfigurationException : Exception
    {
        public ConsulConfigurationException() { }
        public ConsulConfigurationException(string message) : base(message) { }
        public ConsulConfigurationException(string message, Exception inner) : base(message, inner) { }
#if !(CORECLR || PORTABLE || PORTABLE40)
        protected ConsulConfigurationException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
#endif
    }
}
