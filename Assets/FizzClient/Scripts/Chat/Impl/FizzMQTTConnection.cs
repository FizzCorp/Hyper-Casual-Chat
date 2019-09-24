using Fizz.Common;
using System;
using System.Threading;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Fizz.Chat.Impl
{
    public class FizzMqttConnectException : Exception
    {
        public byte Code { get; private set; }

        public FizzMqttConnectException (byte code, string message) : base (message)
        {
            Code = code;
        }
    }

    public class FizzMqttAuthException : FizzMqttConnectException
    {
        public FizzMqttAuthException () : base (5, "invalid_credentials")
        { }
    }

    public class FizzMqttDisconnectedArgs
    {
        public bool ClientWasConnected { get; private set; }
        public Exception Exception { get; private set; }

        public FizzMqttDisconnectedArgs (bool clientWasConnected, Exception ex)
        {
            ClientWasConnected = clientWasConnected;
            Exception = ex;
        }
    }

    public interface IFizzMqttConnection
    {
		bool IsConnected { get; }
        Action<object, bool> Connected { set; get; }
        Action<object, FizzMqttDisconnectedArgs> Disconnected { set; get; }
        Action<object, byte[]> MessageReceived { set; get; }

        void ConnectAsync ();
        void DisconnectAsync ();
    }

    public class FizzMQTTConnection : IFizzMqttConnection
    {
        private static readonly FizzException ERROR_INVALID_CLIENT_ID = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_client_id");
        private static readonly FizzException ERROR_INVALID_USERNAME = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_username");
        private static readonly FizzException ERROR_INVALID_PASSWORD = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_password");
        private static readonly FizzException ERROR_INVALID_DISPATCHER = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");

        // TODO: make this exponential backoff
        private static int RETRY_DELAY_MS = 10 * 1000;

        private MqttClient _client;
        private readonly string _username;
        private readonly string _password;
        private readonly string _clientId;
        private readonly bool _retry;
        private readonly bool _cleanSession;
        private bool _manualDisconnect = false;
        private readonly IFizzActionDispatcher _dispatcher;
		public bool IsConnected { get { return (_client == null)? false : _client.IsConnected; } }

        // TODO: make this thread safe using Interlocked.Add/Interlocked.Remove
        public Action<object, bool> Connected { set; get; }
        public Action<object, FizzMqttDisconnectedArgs> Disconnected { set; get; }
        public Action<object, byte[]> MessageReceived { set; get; }

        public FizzMQTTConnection (string username,
                                  string password,
                                  string clientId,
                                  bool retry,
                                  bool cleanSession,
                                  IFizzActionDispatcher dispatcher)
        {
            if (string.IsNullOrEmpty (clientId))
            {
                throw ERROR_INVALID_CLIENT_ID;
            }
            if (string.IsNullOrEmpty (username))
            {
                throw ERROR_INVALID_USERNAME;
            }
            if (string.IsNullOrEmpty (password))
            {
                throw ERROR_INVALID_PASSWORD;
            }
            if (dispatcher == null)
            {
                throw ERROR_INVALID_DISPATCHER;
            }

            _clientId = clientId;
            _username = username;
            _password = password;
            _retry = retry;
            _cleanSession = cleanSession;
            _dispatcher = dispatcher;

            _client = new MqttClient (
                            FizzConfig.MQTT_HOST_ENDPOINT,
                            FizzConfig.MQTT_USE_TLS ? 8883 : 1883,
                            FizzConfig.MQTT_USE_TLS,
                            null, null,
                            FizzConfig.MQTT_USE_TLS ? MqttSslProtocols.SSLv3 : MqttSslProtocols.None
                        );

            _client.ConnectionClosed += (sender, args) =>
            {
                OnDisconnected (true, null);
            };

            _client.MqttMsgPublishReceived += OnMqttMessageReceived;
        }

        public void ConnectAsync ()
        {
            ThreadPool.QueueUserWorkItem (payload =>
            {
                try
                {
                    byte ret = _client.Connect (_clientId, _username, _password, _cleanSession, 30);
                    if (ret == 0)
                    {
                        if (Connected != null)
                        {
                            _dispatcher.Post (() => Connected.Invoke (this, _client.SessionPresent));
                        }
                    }
                    else
                    {
                        switch (ret)
                        {
                            case 5:
                                OnDisconnected (false, new FizzMqttAuthException ());
                                break;
                            default:
                                OnDisconnected (false, new FizzMqttConnectException (ret, "connect_failed"));
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    OnDisconnected (false, ex);
                }
            });
        }

        public void DisconnectAsync ()
        {
            if (_client == null || !_client.IsConnected)
            {
                return;
            }

            _manualDisconnect = true;

            _client.Disconnect ();
        }

        private void OnDisconnected (bool clientConnected, Exception ex)
        {
            if (Disconnected != null)
            {
                _dispatcher.Post (() => Disconnected.Invoke (this, new FizzMqttDisconnectedArgs (false, ex)));
            }

            if (_manualDisconnect)
            {
                _manualDisconnect = false;
                return;
            }

            if (!_retry)
            {
                return;
            }

            _dispatcher.Delay (RETRY_DELAY_MS, () =>
            {
                try
                {
                    if (_client != null)
                    {
                        ConnectAsync ();
                    }
                }
                catch
                {
                    FizzLogger.E ("Unable to reconnect to Fizz event service.");
                }
            });
        }

        private void OnMqttMessageReceived (object sender, MqttMsgPublishEventArgs msg)
        {
            if (MessageReceived != null)
            {
                _dispatcher.Post (() => MessageReceived.Invoke (this, msg.Message));
            }
        }
    }
}
