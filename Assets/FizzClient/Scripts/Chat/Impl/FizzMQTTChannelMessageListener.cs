using System;
using System.Collections.Generic;
using System.Text;
using Fizz.Common;
using Fizz.Common.Json;

namespace Fizz.Chat.Impl
{
    public class FizzMQTTChannelMessageListener : IFizzChannelMessageListener
    {
        private static readonly FizzException ERROR_INVALID_APP_ID = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_app_id");
        private static readonly FizzException ERROR_INVALID_DISPATCHER = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");
        private static readonly FizzException ERROR_CONNECTION_FAILED = new FizzException (FizzError.ERROR_REQUEST_FAILED, "request_failed");
        private static readonly FizzException ERROR_AUTH_FAILED = new FizzException (FizzError.ERROR_AUTH_FAILED, "auth_failed");

		public bool IsConnected { get { return (_connection == null)? false : _connection.IsConnected; } }
        public Action<bool> OnConnected { get; set; }
        public Action<FizzException> OnDisconnected { get; set; }
        public Action<FizzChannelMessage> OnMessagePublished { get; set; }
        public Action<FizzChannelMessage> OnMessageUpdated { get; set; }
        public Action<FizzChannelMessage> OnMessageDeleted { get; set; }

        protected string _userId;
        protected IFizzMqttConnection _connection;
        protected FizzSessionRepository _sessionRepo;

        private readonly object synclock = new object ();
        private readonly IFizzActionDispatcher _dispatcher;

        public FizzMQTTChannelMessageListener (string appId,
                                              IFizzActionDispatcher dispatcher)
        {
            if (string.IsNullOrEmpty (appId))
            {
                throw ERROR_INVALID_APP_ID;
            }
            if (dispatcher == null)
            {
                throw ERROR_INVALID_DISPATCHER;
            }

            _dispatcher = dispatcher;
        }

        public void Open (string userId, FizzSessionRepository sessionRepository)
        {
            if (_connection != null)
            {
                return;
            }

            if (string.IsNullOrEmpty (userId))
            {
                throw FizzException.ERROR_INVALID_USER_ID;
            }
            if (sessionRepository == null)
            {
                throw FizzException.ERROR_INVALID_SESSION_REPOSITORY;
            }

            _userId = userId;
            _sessionRepo = sessionRepository;
            _sessionRepo.OnSessionUpdate += OnSessionUpdate;

            lock (synclock)
            {
                _connection = CreateConnection ();
                _connection.Connected += OnConnConnected;
                _connection.Disconnected += OnConnDisconnected;
                _connection.MessageReceived += OnMessage;
                _connection.ConnectAsync ();
            }

            _closeCallback = null;
        }

        private Action _closeCallback;
        public void Close (Action cb)
        {
            _closeCallback = cb;
            if (_connection == null)
            {
                FizzUtils.DoCallback (_closeCallback);
                _closeCallback = null;
                return;
            }

            lock (synclock)
            {
                _sessionRepo.OnSessionUpdate -= OnSessionUpdate;
                _sessionRepo = null;
                var conn = _connection;
                _connection = null;
                conn.DisconnectAsync ();
            }
        }

        protected virtual IFizzMqttConnection CreateConnection ()
        {
            return new FizzMQTTConnection (_userId,
                                          _sessionRepo.Session._token,
                                          clientId: _sessionRepo.Session._subscriberId,
                                          retry: true,
                                          cleanSession: false,
                                          dispatcher: _dispatcher);
        }

        private void OnSessionUpdate ()
        {
            try
            {
                if (_connection != null)
                {
                    var conn = _connection;
                    _connection = null;
                    conn.DisconnectAsync ();
                }

                _connection = CreateConnection ();
                _connection.Connected += OnConnConnected;
                _connection.Disconnected += OnConnDisconnected;
                _connection.MessageReceived += OnMessage;
                _connection.ConnectAsync ();
            }
            catch (Exception e)
            {
                FizzLogger.E ("Reconnect Session " + e);
            }
        }

        private void OnConnConnected (object sender, bool sessionPresent)
        {
            if (_connection != null)
            {
                FizzLogger.W ("Received connected event for old connection.");
            }

            FizzLogger.D ("MQTT - OnConnected: " + sessionPresent);

            if (OnConnected != null)
            {
                OnConnected.Invoke (!sessionPresent);
            }
        }

        private void OnConnDisconnected (object sender, FizzMqttDisconnectedArgs args)
        {
            if (_connection != null)
            {
                FizzLogger.W ("Received disconnected event for old connection.");
            }

            FizzLogger.D ("MQTT - OnDisconnected: " + args.ClientWasConnected.ToString ());

            if (OnDisconnected != null)
            {
                if (args.Exception == null)
                {
                    OnDisconnected.Invoke (null);
                }
                else
                {
                    if (args.Exception.GetType () == typeof (FizzMqttAuthException))
                    {
                        OnDisconnected.Invoke (ERROR_AUTH_FAILED);
                        if (_sessionRepo != null)
                        {
                            _sessionRepo.FetchToken (null);
                        }
                    }
                    else
                    {
                        OnDisconnected.Invoke (ERROR_CONNECTION_FAILED);
                    }
                }
            }

            if (_closeCallback != null)
            {
                FizzUtils.DoCallback (_closeCallback);
            }
        }

        private void OnMessage (object sender, byte[] messagePayload)
        {
            try
            {
                string payload = Encoding.UTF8.GetString (messagePayload);
                FizzTopicMessage message = new FizzTopicMessage (payload);

                FizzLogger.D ("OnMessage with id: " + message.Id);

                switch (message.Type)
                {
                    case "CMSGP":
                        if (OnMessagePublished != null)
                        {
                            OnMessagePublished.Invoke (AdaptTo (message));
                        }
                        break;
                    case "CMSGU":
                        if (OnMessageUpdated != null)
                        {
                            OnMessageUpdated.Invoke (AdaptTo (message));
                        }
                        break;
                    case "CMSGD":
                        if (OnMessageDeleted != null)
                        {
                            OnMessageDeleted.Invoke (AdaptTo (message));
                        }
                        break;
                    default:
                        FizzLogger.W ("unrecognized packet received: " + payload);
                        break;
                }
            }
            catch
            {
                FizzLogger.W ("received invalid message: " + messagePayload);
            }
        }

        private FizzChannelMessage AdaptTo (FizzTopicMessage message)
        {
            JSONNode payload = JSONNode.Parse (message.Data);
            JSONClass translationData = payload["translations"].AsObject;
            IDictionary<string, string> translations = null;

            if (translationData != null)
            {
                translations = new Dictionary<string, string> ();
                foreach (string key in translationData.Keys)
                {
                    translations.Add (key, translationData[key]);
                }
            }

            Dictionary<string, string> dataDict = null;
            if (payload["data"] != null)
            {
                JSONNode messageData = JSONNode.Parse (payload["data"]);
                if (messageData != null)
                {
                    dataDict = new Dictionary<string, string> ();
                    foreach (string key in messageData.Keys)
                    {
                        dataDict.Add (key, messageData[key]);
                    }
                }
            }

            return new FizzChannelMessage (
                message.Id,
                message.From,
                payload["nick"],
                payload["to"],
                payload["body"],
                dataDict,
                translations,
                message.Created
            );
        }
    }
}
