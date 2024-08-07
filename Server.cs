using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Celin.AIS
{
	/// <summary>
	/// E1/JDE AIS Server Class.
	/// </summary>
	public class Server
    {
        public bool HasBasicAuthentication => Client.DefaultRequestHeaders.Authorization != null;
        public string BaseUrl { get; set; }
        public static readonly JsonSerializerOptions JsonOutputOptions = new JsonSerializerOptions
        {
            Converters =
            {
                new DateJsonConverter(),
                new UTimeJsonConverter(),
                new DynamicJsonConverter(),
                new GridRowJsonConverter(),
            },
            PropertyNameCaseInsensitive = true,
        };
        protected ILogger Logger { get; }
        readonly JsonSerializerOptions jsonInputOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
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
        public AuthRequest AuthRequest { get; protected set; } = new AuthRequest { deviceName = "celin", requiredCapabilities = "grid,processingOption" };
        public void SetBasicAuthentication(string username, string password) =>
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
        /// <summary>
        /// Submit Default Configuration Request
        /// </summary>
        /// <param name="cancel">Cancellation object</param>
        /// <returns>Success object</returns>
        public async Task<JsonElement> DefaultConfigurationAsync(CancellationToken cancel = default)
        {
            HttpResponseMessage responseMessage;
            var defaultConfig = new DefaultConfig();
            try
            {
                responseMessage = await Client.GetAsync(BaseUrl + defaultConfig.SERVICE, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", defaultConfig.SERVICE, responseMessage.ReasonPhrase);
            if (responseMessage.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<JsonElement>(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            }
            else
            {
                Logger?.LogTrace(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                Logger?.LogError(responseMessage.ReasonPhrase);
                throw new Exception(responseMessage.ReasonPhrase);
            }
        }
        public async Task AuthenticateBasicAsync(string username, string password, CancellationToken cancel = default)
        {
            AuthResponse = null;
            HttpResponseMessage responseMessage;
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(BaseUrl + AuthRequest.SERVICE),
                Method = HttpMethod.Post
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            requestMessage.Content = new StringContent(JsonSerializer.Serialize(AuthRequest, jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.SendAsync(requestMessage, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug(responseMessage.ReasonPhrase);
            if (responseMessage.IsSuccessStatusCode)
            {
                AuthResponse = JsonSerializer.Deserialize<AuthResponse>(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                Logger?.LogTrace(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            }
            else
            {
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Authenticate this instance with JWT
        /// </summary>
        /// <param name="Jason Web Token"></param>
        /// <param name="Cancellation Token"></param>
        /// <returns></returns>
        public async Task AuthenticateAsync(string jwt, CancellationToken cancel = default)
        {
            AuthResponse = null;
            HttpResponseMessage responseMessage;
            var requestMessage = new HttpRequestMessage
            {
                RequestUri = new Uri(BaseUrl + AuthRequest.SERVICE),
                Method = HttpMethod.Post
            };
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
            requestMessage.Content = new StringContent(string.Empty, Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.SendAsync(requestMessage, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug(responseMessage.ReasonPhrase);
            if (responseMessage.IsSuccessStatusCode)
            {
                AuthResponse = JsonSerializer.Deserialize<AuthResponse>(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                Logger?.LogTrace(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            }
            else
            {
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Authenticate this instance.
        /// </summary>
        /// <remarks>Sets the AuthResponse property if successful.</remarks>
        public async Task AuthenticateAsync(CancellationToken cancel = default)
        {
            AuthResponse = null;
            HttpResponseMessage responseMessage;
            HttpContent content = new StringContent(JsonSerializer.Serialize(AuthRequest, jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + AuthRequest.SERVICE, content, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", content.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(await content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            if (responseMessage.IsSuccessStatusCode)
            {
                AuthResponse = JsonSerializer.Deserialize<AuthResponse>(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                Logger?.LogTrace(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                return;
            }
            else
            {
                Logger?.LogTrace(await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                throw new AisException(responseMessage);
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
                var content = new StringContent(JsonSerializer.Serialize(logout, jsonInputOptions), Encoding.UTF8, mediaType);
                Logger?.LogTrace(await content.ReadAsStringAsync().ConfigureAwait(false));
                await Client.PostAsync(BaseUrl + logout.SERVICE, content).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogDebug(e.Message);
            }
            AuthResponse = null;
        }
        /// <summary>
        /// Validate AIS Token
        /// </summary>
        /// <param name="touch">Renew token</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>True/False</returns>
        public async Task<bool> IsValidSessionAsync(bool touch = false, CancellationToken cancel = default)
        {
            if (AuthResponse == null)
            {
                return false;
            }
            HttpResponseMessage responseMessage;
            var request = new TokenValidationRequest
            {
                deviceName = AuthRequest.deviceName,
                token = AuthResponse.userInfo.token,
                touch = touch
            };
            var content = new StringContent(JsonSerializer.Serialize(request, jsonInputOptions), Encoding.UTF8, mediaType);
            Logger?.LogTrace(await content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e, nameof(IsValidSessionAsync));
                throw;
            }
            var body = await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
            Logger?.LogDebug("{0}\n{1}", content.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(body);
            if (responseMessage.IsSuccessStatusCode)
            {
                try
                {
                    var result = JsonSerializer.Deserialize<TokenValidationResponse>(body);
                    return result.isValidSession;
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, nameof(JsonSerializer.Deserialize));
                    throw;
                }
            }
            else
            {
                throw new AisException(responseMessage);
            }
        }
        public async Task<T> DataRequestAsync<T>(string subject, string[] fields = default,
            (string,string,string)[] filters = default,
            string filterType = default,
            string limit = default,
            string[] sort = default,
            CancellationToken cancel = default) where T : new()
        {
            HttpResponseMessage responseMessage;
            string[] auth = AuthResponse == null
            ? HasBasicAuthentication
                ? new[] { "" }
                : new[] { $"$username={AuthRequest.username}", $"$password={AuthRequest.password}" }
            : new[] { $"$token={AuthResponse.userInfo.token}", $"$device={AuthRequest.deviceName}" };

            var request = string.Join('&', (fields == null ? Array.Empty<string>() : fields)
                .Select(f => $"$field={f.ToUpper()}")
                .Concat((filters == null ? Array.Empty<(string, string, string)>() : filters)
                    .Select(f => $"$filter={f.Item1.ToUpper()} {f.Item2.ToUpper()} {f.Item3}"))
                .Concat(filterType == null ? Array.Empty<string>() : new[] {$"$filterType={filterType}"})
                .Concat(limit == null ? Array.Empty<string>() : new[] {$"$limit={limit}"})
                .Concat((sort == null ? Array.Empty<string>() : sort)
                    .Select(s => $"$sort={s}"))
                .Concat(auth));

            try
            {
                responseMessage = await Client.GetAsync($"{BaseUrl}dataservice/{(subject[0].Equals('F') ? "table" : "view")}/{subject.ToUpper()}?{request}", cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            var body = await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(body);
                try
                {
                    if (string.IsNullOrEmpty(body))
                    {
                        return default(T);
                    }
                    T result = JsonSerializer.Deserialize<T>(body, JsonOutputOptions);
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
                Logger?.LogTrace(body);
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Submit an AIS Form Request.
        /// </summary>
        /// <param name="request">The Request object.</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Success Response object</returns>
        /// <typeparam name="T">Response object type.</typeparam>
        public async Task<T> RequestAsync<T>(Service request, CancellationToken cancel = default) where T : new()
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            if (AuthResponse == null)
            {
                if (!HasBasicAuthentication)
                {
                    request.username = AuthRequest.username;
                    request.password = AuthRequest.password;
                }
            }
            else
            {
                request.token = AuthResponse.userInfo.token;
            }
            var content = new StringContent(JsonSerializer.Serialize(request, request.GetType(), jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(await content.ReadAsStringAsync(cancel).ConfigureAwait(false));
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(await content.ReadAsStringAsync(cancel).ConfigureAwait(false));
            var body = await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(body);
                try
                {
                    if (string.IsNullOrEmpty(body))
                    {
                        return default(T);
                    }
                    T result = JsonSerializer.Deserialize<T>(body, JsonOutputOptions);
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
                Logger?.LogTrace(body);
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Submit an AIS Next Request
        /// </summary>
        /// <typeparam name="T">The Returned Type</typeparam>
        /// <param name="href">POS Request</param>
        /// <param name="cancel">Cancel Token</param>
        /// <returns></returns>
        public async Task<T> RequestAsync<T>(string href, CancellationToken cancel = default)
        {
            HttpResponseMessage responseMessage;
            try
            {
                responseMessage = await Client.PostAsync(href, null, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(href);
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", href, responseMessage.ReasonPhrase);
            var body = await responseMessage.Content.ReadAsStringAsync(cancel).ConfigureAwait(false);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(body);
                try
                {
                    if (string.IsNullOrEmpty(body))
                    {
                        return default(T);
                    }
                    T result = JsonSerializer.Deserialize<T>(body, JsonOutputOptions);
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
                Logger?.LogTrace(body);
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment Upload Request
        /// </summary>
        /// <param name="request">The Request Object</param>
        /// <param name="file">File to Upload</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Success object</returns>
        public async Task<FileAttachmentResponse> RequestAsync(MoUpload request, StreamContent file, CancellationToken cancel = default)
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            if (AuthResponse == null)
            {
                if (!HasBasicAuthentication)
                {
                    request.username = AuthRequest.username;
                    request.password = AuthRequest.password;
                }
            }
            else
            {
                request.token = AuthResponse.userInfo.token;
            }
            MultipartFormDataContent content = new MultipartFormDataContent
            {
                { new StringContent(JsonSerializer.Serialize(request, jsonInputOptions), Encoding.UTF8, mediaType), "moAdd" },
                { file, "file" }
            };
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync(cancel).Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync(cancel).Result);
                try
                {
                    var result = JsonSerializer.Deserialize<FileAttachmentResponse>(responseMessage.Content.ReadAsStringAsync(cancel).Result);
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
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync(cancel).Result);
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment Download Request
        /// </summary>
        /// <param name="request">The Request object</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Success object</returns>
        public async Task<Stream> RequestAsync(MoDownload request, CancellationToken cancel = default)
        {
            HttpResponseMessage responseMessage;
            request.deviceName = AuthRequest.deviceName;
            if (AuthResponse == null)
            {
                if (!HasBasicAuthentication)
                {
                    request.username = AuthRequest.username;
                    request.password = AuthRequest.password;
                }
            }
            else
            {
                request.token = AuthResponse.userInfo.token;
            }
            var content = new StringContent(JsonSerializer.Serialize(request, jsonInputOptions), Encoding.UTF8, mediaType);
            try
            {
                responseMessage = await Client.PostAsync(BaseUrl + request.SERVICE, content, cancel).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogError(e.Message);
                throw;
            }
            Logger?.LogDebug("{0}\n{1}", request.ToString(), responseMessage.ReasonPhrase);
            Logger?.LogTrace(content.ReadAsStringAsync(cancel).Result);
            if (responseMessage.IsSuccessStatusCode)
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync(cancel).Result);
                return responseMessage.Content.ReadAsStreamAsync().Result;
            }
            else
            {
                Logger?.LogTrace(responseMessage.Content.ReadAsStringAsync(cancel).Result);
                throw new AisException(responseMessage);
            }
        }
        /// <summary>
        /// Submit File Attachment List Request
        /// </summary>
        /// <param name="request">The Request object</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Success object</returns>
        public Task<AttachmentListResponse> RequestAsync(MoList request, CancellationToken cancel = default)
            => RequestAsync<AttachmentListResponse>(request, cancel);
        /// <summary>
        /// Submit Media Object Request
        /// </summary>
        /// <param name="request">The MO Request object</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>Success object</returns>
        public Task<AttachmentResponse> RequestAsync(MoRequest request, CancellationToken cancel = default)
            => RequestAsync<AttachmentResponse>(request, cancel);
        /// <summary>
        /// Submit UBE Discovery
        /// </summary>
        /// <param name="request">The DiscoveryUBERequest object</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>DiscoverUBEResponse</returns>
        public Task<DiscoveryUBEResponse> RequestAsync(DiscoverUBERequest request, CancellationToken cancel = default)
            => RequestAsync<DiscoveryUBEResponse>(request, cancel);
        /// <summary>
        /// Submit UBE
        /// </summary>
        /// <param name="request">UBERequest object</param>
        /// <param name="cancel">Cancellation token</param>
        /// <returns>UBEResponse</returns>
        public Task<UBEResponse> RequestAsync(UBERequest request, CancellationToken cancel = default)
            => RequestAsync<UBEResponse>(request, cancel);
        /// <summary>
        /// Get Ube Status
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public Task<UBEResponse> RequestAsync(StatusUBERequest request, CancellationToken cancel = default)
            => RequestAsync<UBEResponse>(request, cancel);
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
