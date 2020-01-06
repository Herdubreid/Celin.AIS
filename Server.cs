using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Celin.AIS
{
    /// <summary>
    /// E1/JDE AIS Server Class.
    /// </summary>
    public class Server
    {
        public string BaseUrl { get; set; }
        protected ILogger Logger { get; }
        readonly JsonSerializerOptions jsonOutputOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new DateJsonConverter()
            }
        };
        readonly JsonSerializerOptions jsonInputOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            Converters =
            {
                new ActionJsonConverter(),
                new GridActionJsonConverter()
            }
        };
        readonly string mediaType = "application/json";
        HttpClient Client { get; }
        /// <summary>
        /// Holds the Authentication Response Parameters.
        /// </summary>
        /// <value>The Authentication Response.</value>
        public AuthResponse AuthResponse { get; set; } = null;
        /// <summary>
        /// Holds the Authentication Request Parameters.
        /// </summary>
        /// <value>The Authentication Request.</value>
        public AuthRequest AuthRequest { get; set; } = new AuthRequest { deviceName = "celin", requiredCapabilities = "grid,processingOption" };
        /// <summary>
        /// Submit Default Configuration Request
        /// </summary>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success object</returns>
        public async Task<JsonElement> DefaultConfiguration(CancellationTokenSource cancel = null)
        {
            HttpResponseMessage responseMessage;
            var defaultConfig = new DefaultConfig();
            try
            {
                responseMessage = await Client.GetAsync(BaseUrl + defaultConfig.SERVICE, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", defaultConfig.SERVICE, responseMessage.ReasonPhrase);
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<JsonElement>(responseMessage.Content.ReadAsStringAsync().Result);
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                Logger?.LogError(responseMessage.ReasonPhrase);
                throw new Exception(responseMessage.ReasonPhrase);
            }
        }
        /// <summary>
        /// Authenticate this instance.
        /// </summary>
        /// <remarks>Sets the AuthResponse property if successful.</remarks>
        public async Task AuthenticateAsync(CancellationTokenSource cancel = null)
        {
            AuthResponse = null;
            HttpResponseMessage responseMessage;
            HttpContent content = new StringContent(JsonSerializer.Serialize(AuthRequest, jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + AuthRequest.SERVICE, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", AuthRequest.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                AuthResponse = JsonSerializer.Deserialize<AuthResponse>(responseMessage.Content.ReadAsStringAsync().Result);
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                return;
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                throw new HttpWebException(responseMessage);
            }
        }
        /// <summary>
        /// Logout this instance.
        /// </summary>
        /// <remarks>Clears the AuthResponse property if successful.</remarks>
        public async Task LogoutAsync()
        {
            try
            {
                var logout = new LogoutRequest
                {
                    token = AuthResponse?.userInfo.token
                };
                HttpContent content = new StringContent(JsonSerializer.Serialize(logout, jsonInputOptions), Encoding.UTF8, mediaType);
                Logger?.LogTrace(content.ReadAsStringAsync().Result);
                await Client.PostAsync(BaseUrl + logout.SERVICE, content);
            }
            catch (Exception e)
            {
                Logger?.LogDebug(e.Message);
            }
            AuthResponse = null;
        }
        /// <summary>
        /// Submit an AIS Form Request.
        /// </summary>
        /// <param name="request">The Request object.</param>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success Response object</returns>
        /// <typeparam name="T">Response object type.</typeparam>
        public async Task<T> RequestAsync<T>(Request request, CancellationTokenSource cancel = null) where T : new()
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            request.token = AuthResponse?.userInfo.token;
            var content = new StringContent(JsonSerializer.Serialize(request, request.GetType(), jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                try
                {
                    T result = JsonSerializer.Deserialize<T>(responseMessage.Content.ReadAsStringAsync().Result, jsonOutputOptions);
                    return result;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e.Message);
                    throw;
                }
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                throw new HttpWebException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment Upload Request
        /// </summary>
        /// <param name="request">The Request Object</param>
        /// <param name="file">File to Upload</param>
        /// <param name="cancel">Cancellation Object</param>
        /// <returns>Success object</returns>
        public async Task<FileAttachmentResponse> RequestAsync(MoUpload request, StreamContent file, CancellationTokenSource cancel = null)
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            request.token = AuthResponse?.userInfo.token;
            MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new StringContent(JsonSerializer.Serialize(request, jsonInputOptions), Encoding.UTF8, mediaType), "moAdd" },
                { file, "file" }
            };
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                try
                {
                    var result = JsonSerializer.Deserialize<FileAttachmentResponse>(responseMessage.Content.ReadAsStringAsync().Result);
                    return result;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e.Message);
                    throw;
                }
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                throw new HttpWebException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment Download Request
        /// </summary>
        /// <param name="request">The Request object</param>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success object</returns>
        public async Task<Stream> RequestAsync(MoDownload request, CancellationTokenSource cancel = null)
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            request.token = (AuthResponse ?? throw new ArgumentNullException()).userInfo.token;
            var content = new StringContent(JsonSerializer.Serialize(request, jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel == null ? CancellationToken.None : cancel.Token);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync().Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                return responseMessage.Content.ReadAsStreamAsync().Result;
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync().Result);
                throw new HttpWebException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment List Request
        /// </summary>
        /// <param name="request">The Request object</param>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success object</returns>
        public async Task<AttachmentListResponse> RequestAsync(MoList request, CancellationTokenSource cancel = null)
        {
            return await RequestAsync<AttachmentListResponse>(request, cancel);
        }
        /// <summary>
        /// Submit Media Object Request
        /// </summary>
        /// <param name="request">The MO Request object</param>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success object</returns>
        public async Task<AttachmentResponse> RequestAsync(MoRequest request, CancellationTokenSource cancel = null)
        {
            return await RequestAsync<AttachmentResponse>(request, cancel);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Celin.AIS.Server"/> class.
        /// </summary>
        /// <param name="baseUrl">The Url for the AIS Server (for example https://e1.celin.io:9302/jderest/)</param>
        /// <param name="logger">(Optional) ILogger Instance</param>
        /// <param name="httpClient">(Optional) HttpClient Instance</param>
        public Server(string baseUrl, ILogger logger = null, HttpClient httpClient = null)
        {
            Client = httpClient ?? new HttpClient();
            Logger = logger;
            BaseUrl = baseUrl;
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            Logger?.LogDebug("Celin.AIS.Service Instantiated, baseUrl: {0}", baseUrl);
        }
    }
}
