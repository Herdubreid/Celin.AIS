using System;
using System.Text;
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
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public string BaseUrl { get; set; }
        private readonly string mediaType = "application/json";
        private HttpClient Client { get; } = new HttpClient();
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
        /// <remarks>Sets the AuthResponse member if successful.</remarks>
        public bool Authenticate()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(this.AuthRequest), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                Task<HttpResponseMessage> responseMessage = this.Client.PostAsync(this.BaseUrl + this.AuthRequest.SERVICE, content);
                logger.Trace(responseMessage.Result);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    this.AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(responseMessage.Result.Content.ReadAsStringAsync().Result);
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.Trace(e);
                logger.Error("Authenticate:\n{0}", e.Message);
            }
            this.AuthResponse = null;
            return false;
        }
        /// <summary>
        /// Submit an AIS Request.
        /// </summary>
        /// <returns>A Tuple with success flag and the response object.</returns>
        /// <param name="request">The Request object.</param>
        /// <typeparam name="T">Response object type.</typeparam>
        public Tuple<bool, T> Request<T>(Request request) where T :  new()
        {
            request.deviceName = this.AuthRequest.deviceName;
            request.token = this.AuthResponse.userInfo.token;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, mediaType);
            logger.Trace(content.ReadAsStringAsync().Result);
            try
            {
                Task<HttpResponseMessage> responseMessage = this.Client.PostAsync(this.BaseUrl + request.SERVICE, content);
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
                logger.Trace(e);
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
            this.BaseUrl = baseUrl;
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
