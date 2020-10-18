using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using NLog;
using PoissonSoft.BinanceApi.Contracts.Exceptions;
using PoissonSoft.BinanceApi.Contracts.Serialization;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.Transport.Rest
{
    internal sealed class RestClient : IDisposable
    {
        private readonly ILogger logger;
        private readonly Throttler throttler;
        private readonly string userFriendlyName;

        private readonly HttpClient httpClient;
        private readonly bool useSignature;

        public const string METHOD_GET = "GET";
        public const string METHOD_POST = "POST";

        private readonly JsonSerializerSettings serializerSettings;

        public RestClient(ILogger logger, string baseEndpoint, EndpointSecurityType[] securityTypes,
            BinanceApiClientCredentials credentials, Throttler throttler)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.throttler = throttler ?? throw new ArgumentNullException(nameof(throttler));

            var useApiKey = securityTypes?.Any(x => x != EndpointSecurityType.None) ?? false;
            useSignature = securityTypes?.Any(x =>
                x == EndpointSecurityType.Trade || x == EndpointSecurityType.Margin ||
                x == EndpointSecurityType.UserData) ?? false;

            userFriendlyName = $"{nameof(RestClient)} ({baseEndpoint})";

            serializerSettings = new JsonSerializerSettings
            {
                Context = new StreamingContext(StreamingContextStates.All,
                    new SerializationContext {Logger = logger})
            };

            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Proxy = ProxyHelper.CreateProxy(credentials)
            };

            baseEndpoint = baseEndpoint.Trim();
            if (!baseEndpoint.EndsWith("/")) baseEndpoint += '/';
            httpClient = new HttpClient(httpClientHandler, true)
            {
                Timeout = TimeSpan.FromSeconds(20),
                BaseAddress = new Uri(baseEndpoint),
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (useApiKey)
            {
                // ReSharper disable once StringLiteralTypo
                httpClient.DefaultRequestHeaders.Add("X-MBX-APIKEY", credentials.ApiKey);
            }
        }


        /// <summary>
        /// Выполнить запрос
        /// </summary>
        /// <typeparam name="TResp">Тип возвращаемого значения</typeparam>
        /// <param name="urlPath">Путь к ресурсу (без базового адреса ендпоинта)</param>
        /// <param name="method"></param>
        /// <param name="requestWeight">Вес запроса в баллах</param>
        /// <param name="highPriority">Признак того, что высокого приоритета запроса</param>
        /// <param name="isOrderRequest">Это запрос на работу с ордерами (размещение/удаление/модификация)</param>
        /// <param name="getParams"></param>
        /// <param name="postObject"></param>
        /// <returns></returns>
        public TResp MakeRequest<TResp>(string method, string urlPath, 
            int requestWeight, bool highPriority, bool isOrderRequest,
            Dictionary<string, string> getParams = null, object postObject = null)
        {
            throttler.ThrottleRest(requestWeight, highPriority, isOrderRequest);
            

            void checkResponse(HttpResponseMessage resp, string body)
            {
                throttler.ApplyRestResponseHeaders(resp.Headers);

                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    logger.Error($"{userFriendlyName}. На запрос {urlPath} от сервера получен код ответа {resp.StatusCode}");
                    throw new EndpointCommunicationException(
                        $"{userFriendlyName}. На запрос {urlPath} от сервера получен код ответа {(int)resp.StatusCode} ({resp.StatusCode})\nТело ответа:\n{body}");
                }
            }

            string strResp = null;
            try
            {
                switch (method)
                {
                    case METHOD_POST:
                        using (var content =
                            new StringContent(JsonConvert.SerializeObject(postObject), Encoding.UTF8, "application/json"))
                        {
                            using (var result = httpClient.PostAsync(urlPath, content).Result)
                            {
                                strResp = result.Content.ReadAsStringAsync().Result;
                                checkResponse(result, strResp);
                            }
                        }

                        break;
                    case METHOD_GET:
                        var param = BuildParamStr(getParams);
                        var url = $"{urlPath}{(string.IsNullOrEmpty(param) ? string.Empty : $"?{param}")}";
                        using (var result = httpClient.GetAsync(url).Result)
                        {
                            strResp = result.Content.ReadAsStringAsync().Result;
                            checkResponse(result, strResp);
                        }

                        break;
                }

            }
            catch (EndpointCommunicationException)
            {
                throw;
            }
            catch (HttpRequestException e)
            {
                logger.Error($"{userFriendlyName}. При отправке HTTP-запроса возникло исключение\n{e}");
                throw;
            }
            catch (AggregateException e) when (e.InnerExceptions.Count == 1 &&
                                               e.InnerExceptions[0] is TaskCanceledException)
            {
                var msg = $"{userFriendlyName}. Возникло исключение {nameof(TaskCanceledException)} в связи с истечением таймаута запроса. " +
                          "Возможно, сервер временно не доступен";
                logger.Error(msg);
                throw new TimeoutException(msg, e);
            }
            catch (Exception e)
            {
                logger.Error($"{userFriendlyName}. При запросе данных с сервера возникло исключение\n{e}");
                throw;
            }

            if (string.IsNullOrWhiteSpace(strResp)) return default;
            if (typeof(TResp) == typeof(string)) return (TResp)(object)strResp;

            try
            {
                return JsonConvert.DeserializeObject<TResp>(strResp, serializerSettings);
            }
            catch (Exception e)
            {
                logger.Error($"{userFriendlyName}. Попытка десериализации строки, полученной от сервера вызвала исключение\n" +
                             $"Ответ сервера: {strResp}\n{e}");
                throw;
            }
        }

        private static string BuildParamStr(Dictionary<string, string> paramDic)
        {
            return paramDic?.Any() != true
                ? string.Empty
                : string.Join("&", paramDic.Select(x => $"{x.Key}={HttpUtility.UrlEncode(x.Value)}"));
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }


}
