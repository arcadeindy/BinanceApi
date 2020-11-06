using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using PoissonSoft.BinanceApi.Utils;

namespace PoissonSoft.BinanceApi.Transport.Ws
{
    internal sealed class WebSocketStreamListener: IDisposable
    {
        private readonly BinanceApiClient apiClient;
        private readonly ILogger logger;
        private readonly BinanceApiClientCredentials credentials;

        private ClientWebSocket client;
        private readonly object clientCreationSync = new object();
        private CancellationTokenSource cancellationTokenSource;
        private TaskCompletionSource<object> socketFinishedTcs;

        public string Endpoint { get; private set; }
        public WebSocketState? State => client?.State;

        public event EventHandler OnConnected; 
        public event EventHandler<(WebSocketCloseStatus? CloseStatus, string CloseStatusDescription)> OnConnectionClosed;
        public event EventHandler<string> OnMessage; 


        public WebSocketStreamListener(BinanceApiClient apiClient, BinanceApiClientCredentials credentials)
        {
            this.apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
            logger = apiClient.Logger;
            this.credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        }

        public void Connect(string url)
        {
            Close();

            lock (clientCreationSync)
            {
                client = new ClientWebSocket();
                client.Options.Proxy = ProxyHelper.CreateProxy(credentials);
                cancellationTokenSource = new CancellationTokenSource();
                socketFinishedTcs = new TaskCompletionSource<object>();
                try
                {
                    var t = client.ConnectAsync(new Uri(url), cancellationTokenSource.Token);
                    t.Wait();
                }
                catch
                {
                    socketFinishedTcs.SetResult(null);
                    throw;
                }
                OnConnected?.Invoke(this, EventArgs.Empty);
                _ = StartListening();
            }
        }

        public void Close()
        {
            lock (clientCreationSync)
            {
                if (client == null) return;

                Endpoint = null;
                cancellationTokenSource.Cancel();
                client.Abort();
                socketFinishedTcs.Task.Wait();
                client.Dispose();
                cancellationTokenSource?.Dispose();
            }

        }

        public async Task StartListening()
        {
            var buffer = new byte[1024 * 4];
            var closeStatus = WebSocketCloseStatus.NormalClosure;
            var closeStatusDescription = string.Empty;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var msg = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                    if (msg.CloseStatus.HasValue)
                    {
                        closeStatus = msg.CloseStatus.Value;
                        closeStatusDescription = msg.CloseStatusDescription;
                        break;
                    }

                    if (msg.MessageType == WebSocketMessageType.Close)
                    {
                        closeStatus = WebSocketCloseStatus.NormalClosure;
                        closeStatusDescription = "Connection closed by client";
                        break;
                    }

                    if (!msg.EndOfMessage)
                    {
                        closeStatus = WebSocketCloseStatus.MessageTooBig;
                        closeStatusDescription = "Message too big";
                        break;
                    }

                    if (msg.MessageType != WebSocketMessageType.Text)
                    {
                        logger.Warn($"Unsupported type of client message ({msg.MessageType}) was ignored");
                        continue;
                    }

                    // Обработка сообщений от сервера
                    try
                    {
                        var msgContent = Encoding.UTF8.GetString(buffer, 0, msg.Count);
                        ProcessServerMessage(msgContent);
                    }
                    catch (Exception e)
                    {
                        logger.Error($"Исключение при обработке сообщения от клиента\n{e}");
                    }

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception e)
                {
                    closeStatus = WebSocketCloseStatus.InternalServerError;
                    logger.Error($"Разрыв WebSocket-соединения в связи с исключение\n{e}");
                    break;
                }

            }

            if (client.State != WebSocketState.Aborted)
            {
                try
                {
                    await client.CloseAsync(closeStatus, closeStatusDescription, CancellationToken.None);
                }
                catch
                {
                    // ignore
                }
            }

            OnConnectionClosed?.Invoke(this, (client.CloseStatus, client.CloseStatusDescription));

            socketFinishedTcs.SetResult(null);
        }

        public void SendMessage(string msg)
        {
            apiClient.Throttler.ThrottleWs(1);

            var data = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
            client.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private void ProcessServerMessage(string msg)
        {
            OnMessage?.Invoke(this, msg);
        }

        public void Dispose()
        {
            Close();
        }


    }
}
