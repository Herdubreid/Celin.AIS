using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace Celin.AIS
{
    /// <summary>
    /// E1/JDE AIS Server Class.
    /// </summary>
    public class Server
    {
        static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string BaseUrl { get; set; }
        readonly string mediaType = "application/json";
        HttpClient Client { get; } = new HttpClient();
        /// <summary>
        /// Holds the Authentication Response Parameters.
        /// </summary>
        /// <value>The Authentication Response.</value>
        public AuthResponse AuthResponse { get; private set; } = null;
        /// <summary>
        /// Holds the Authentication Request Parameters.
        /// </summary>
        /// <value>The Authentication Request.</value>
        public AuthRequest AuthRequest { get; set; } = new AuthRequest { deviceName = "celin", requiredCapabilities = "grid,processingOption" };
        /// <summary>
        /// Authenticate this instance.
        /// </summary>
        /// <returns>Authentication success flag.</returns>
        /// <remarks>Sets the AuthResponse property if successful.</remarks>
        public async Task AuthenticateAsync(CancellationTokenSource cancel = null)
        {
            AuthResponse = null;
            if (cancel is null) cancel = new CancellationTokenSource();
            HttpContent content = new StringContent(JsonConvert.SerializeObject(AuthRequest), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                HttpResponseMessage responseMessage = await Client.PostAsync(BaseUrl + AuthRequest.SERVICE, content, cancel.Token);
                logger.Trace(responseMessage);
                if (responseMessage.IsSuccessStatusCode)
                {
                    AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(responseMessage.Content.ReadAsStringAsync().Result);
                    return;
                }
                else
                {
                    logger.Warn("Request:\n{0}\n{1}", responseMessage.ReasonPhrase, content.ReadAsStringAsync().Result);
                    throw new HttpWebException(responseMessage);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e);
                logger.Error("Authenticate:\n{0}", e.Message);
                throw e;
            }
        }
        /// <summary>
        /// Logout this instance.
        /// </summary>
        /// <returns>Logut success flag.</returns>
        /// <remarks>Clears the AuthResponse property if successful.</remarks>
        public async Task LogoutAsync()
        {
            try
            {
                var logout = new LogoutRequest
                {
                    token = AuthResponse?.userInfo.token
                };
                HttpContent content = new StringContent(JsonConvert.SerializeObject(logout), Encoding.UTF8, mediaType);
                logger.Trace(content.ReadAsStringAsync().Result);
                HttpResponseMessage responseMessage = await Client.PostAsync(BaseUrl + logout.SERVICE, content);
            }
            catch (Exception e)
            {
                logger.Debug(e);
                logger.Error("Logout:\n{0}", e.Message);
            }
        }
        /// <summary>
        /// Submit an AIS Request.
        /// </summary>
        /// <returns>A Tuple with success flag and the response object.</returns>
        /// <param name="request">The Request object.</param>
        /// <typeparam name="T">Response object type.</typeparam>
        public async Task<T> RequestAsync<T>(Request request, CancellationTokenSource cancel = null) where T :  new()
        {
            if (cancel is null) cancel = new CancellationTokenSource();
            request.deviceName = AuthRequest.deviceName;
            request.token = AuthResponse?.userInfo.token;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                HttpResponseMessage responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel.Token);
                logger.Trace(responseMessage);
                if (responseMessage.IsSuccessStatusCode)
                {
                    T result = JsonConvert.DeserializeObject<T>(responseMessage.Content.ReadAsStringAsync().Result);
                    return result;
                }
                else
                {
                    logger.Warn("Request:\n{0}\n{1}", responseMessage.ReasonPhrase, JsonConvert.SerializeObject(request, Formatting.Indented));
                    throw new HttpWebException(responseMessage);
                }
            }
            catch (Exception e)
            {
                logger.Debug(e);
                throw e;
            }
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Celin.AIS.Server"/> class.
        /// </summary>
        /// <param name="baseUrl">The Url for the AIS Server (for example https://e1.celin.io:9302/jderest/).</param>
        public Server(string baseUrl)
        {
            logger.Debug("BaseUrl: {0}", baseUrl);
            BaseUrl = baseUrl;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
