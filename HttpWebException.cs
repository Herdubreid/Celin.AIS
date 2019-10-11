using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Celin.AIS
{
    /// <summary>
    /// E1 Exception Class
    /// </summary>
    public class HttpWebException : WebException
    {
        public class ContentType
        {
            public string message { get;set; }
            public string exception { get; set; }
            public string timeStamp { get; set; }
        }
        /// <summary>
        /// Content
        /// </summary>
        public ContentType Content { get; }
        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }
        public HttpWebException(HttpResponseMessage message) : base(message.ReasonPhrase)
        {
            HttpStatusCode = message.StatusCode;
            Content = JsonSerializer.Deserialize<ContentType>(message.Content.ReadAsStringAsync().Result);
        }
    }
}
