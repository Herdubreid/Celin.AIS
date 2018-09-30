using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
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
        public bool Authenticate(CancellationTokenSource cancel = null)
        {
            if (cancel is null) cancel = new CancellationTokenSource();
            HttpContent content = new StringContent(JsonConvert.SerializeObject(AuthRequest), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                Task<HttpResponseMessage> responseMessage = Client.PostAsync(BaseUrl + AuthRequest.SERVICE, content, cancel.Token);
                logger.Trace(responseMessage.Result);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(responseMessage.Result.Content.ReadAsStringAsync().Result);
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.Debug(e);
                logger.Error("Authenticate:\n{0}", e.Message);
            }
            AuthResponse = null;
            return false;
        }
        /// <summary>
        /// Logout this instance.
        /// </summary>
        /// <returns>Logut success flag.</returns>
        /// <remarks>Clears the AuthResponse property if successful.</remarks>
        public bool Logout()
        {
            try
            {
                var logout = new LogoutRequest
                {
                    token = AuthResponse.userInfo.token
                };
                HttpContent content = new StringContent(JsonConvert.SerializeObject(logout), Encoding.UTF8, mediaType);
                logger.Trace(content.ReadAsStringAsync().Result);
                Task<HttpResponseMessage> responseMessage = Client.PostAsync(BaseUrl + logout.SERVICE, content);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    AuthResponse = null;
                    return true;
                }
                else
                {
                    logger.Warn("Logout:\n{0}", responseMessage.Result.ReasonPhrase);
                    return false;
                }
            }
            catch (Exception e)
            {
                logger.Debug(e);
                logger.Error("Logout:\n{0}", e.Message);
                return false;
            }
        }
        /// <summary>
        /// Submit an AIS Request.
        /// </summary>
        /// <returns>A Tuple with success flag and the response object.</returns>
        /// <param name="request">The Request object.</param>
        /// <typeparam name="T">Response object type.</typeparam>
        public Tuple<bool, T> Request<T>(Request request, CancellationTokenSource cancel = null) where T :  new()
        {
            if (cancel is null) cancel = new CancellationTokenSource();
            request.deviceName = AuthRequest.deviceName;
            request.token = AuthResponse.userInfo.token;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                Task<HttpResponseMessage> responseMessage = Client.PostAsync(BaseUrl + request.SERVICE, content, cancel.Token);
                logger.Trace(responseMessage.Result);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    T result = JsonConvert.DeserializeObject<T>(responseMessage.Result.Content.ReadAsStringAsync().Result);
                    return new Tuple<bool, T>(true, result);
                }
                else
                {
                    logger.Warn("Request:\n{0}\n{1}", responseMessage.Result.ReasonPhrase, content.ReadAsStringAsync().Result);
                    return new Tuple<bool, T>(false, new T());
                }
            }
            catch (Exception e)
            {
                logger.Debug(e);
                logger.Error("Request:\n{0}\n{1}", e.Message, content.ReadAsStringAsync().Result);
                return new Tuple<bool, T>(false, new T());
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
