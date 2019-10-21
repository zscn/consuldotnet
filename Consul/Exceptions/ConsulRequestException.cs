using System;
using System.Net;

#if !(CORECLR || PORTABLE || PORTABLE40)
using System.Security.Permissions;
using System.Runtime.Serialization;
#endif

namespace Consul.Exceptions
{
    /// <summary>
    /// Represents errors that occur while sending data to or fetching data from the Consul agent.
    /// </summary>
#if !(CORECLR || PORTABLE || PORTABLE40)
    [Serializable]
#endif
    public class ConsulRequestException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public ConsulRequestException() { }
        public ConsulRequestException(string message, HttpStatusCode statusCode) : base(message) { StatusCode = statusCode; }
        public ConsulRequestException(string message, HttpStatusCode statusCode, Exception inner) : base(message, inner) { StatusCode = statusCode; }

#if !(CORECLR || PORTABLE || PORTABLE40)
        protected ConsulRequestException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("StatusCode", StatusCode);
        }
#endif
    }
}
