using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
namespace Celin.AIS
{
    public class Server
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private string BaseUrl { get; }
        private HttpClient Client { get; } = new HttpClient();
        public AuthResponse AuthResponse { get; private set; } = null;
        public AuthRequest AuthRequest { get; private set; } = new AuthRequest { deviceName = "celin", requiredCapabilities = "grid,processingOption" };
        public bool Authenticate()
        {
            HttpContent content = new StringContent(JsonConvert.SerializeObject(this.AuthRequest));
            try
            {
                Task<HttpResponseMessage> responseMessage = this.Client.PostAsync(this.BaseUrl + this.AuthRequest.SERVICE, content);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    this.AuthResponse = JsonConvert.DeserializeObject<AuthResponse>(responseMessage.Result.Content.ReadAsStringAsync().Result);
                    return true;
                }
            }
            catch (System.AggregateException e)
            {
                Server.logger.Error("Authenticate:\n{0}\n{1}", e.Message, content.ReadAsStringAsync().Result);
            }
            this.AuthResponse = null;
            return false;
        }
        public Tuple<bool, T> Request<T>(Request request) where T : FormResponse, new()
        {
            request.deviceName = this.AuthRequest.deviceName;
            request.token = this.AuthResponse.userInfo.token;
            HttpContent content = new StringContent(JsonConvert.SerializeObject(request));
            try
            {
                Task<HttpResponseMessage> responseMessage = this.Client.PostAsync(this.BaseUrl + request.SERVICE, content);
                if (responseMessage.Result.IsSuccessStatusCode)
                {
                    T result = JsonConvert.DeserializeObject<T>(responseMessage.Result.Content.ReadAsStringAsync().Result);
                    return new Tuple<bool, T>(true, result);
                }
                else
                {
                    return new Tuple<bool, T>(false, new T
                    {
                        message = responseMessage.Exception.Message
                    });
                }
            }
            catch (System.AggregateException e)
            {
                Server.logger.Error("Request:\n{0}\n{1}", e.Message, content.ReadAsStringAsync().Result);
                return new Tuple<bool, T>(false, new T
                {
                    message = e.Message
                });
            }
        }
        public Server(string baseUrl)
        {
            Server.logger.Debug("BaseUrl: {0}", baseUrl);
            this.BaseUrl = baseUrl;
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }
    }
}
