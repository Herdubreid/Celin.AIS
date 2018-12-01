using System.Net;
using System.Net.Http;

namespace Celin.AIS
{
    /// <summary>
    /// Exception Class
    /// </summary>
    public class HttpWebException : WebException
    {
        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; }
        public HttpWebException(HttpResponseMessage message) : base(message.ReasonPhrase)
        {
            HttpStatusCode = message.StatusCode;
        }
    }
}
