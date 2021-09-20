using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace Celin.AIS
{
    /// <summary>
    /// E1 Exception Class
    /// </summary>
    public class AisException : InvalidOperationException
    {
        /// <summary>
        /// E1 Error Response
        /// </summary>
        public ErrorResponse ErrorResponse { get; }
        /// <summary>
        /// HttpStatusCode
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; }
        public AisException(HttpResponseMessage message) : base(message.ReasonPhrase)
        {
            HttpStatusCode = message.StatusCode;
            try
            {
                ErrorResponse = JsonSerializer.Deserialize<ErrorResponse>(message.Content.ReadAsStringAsync().Result);
            }
            catch { }
        }
    }
}
