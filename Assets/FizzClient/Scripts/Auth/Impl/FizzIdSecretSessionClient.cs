using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Fizz.Common.Json;

namespace Fizz.Common
{
    public class FizzIdSecretSessionProvider : IFizzSessionProvider
    {
        private static readonly FizzException ERROR_SESSION_CREATION_FAILED = new FizzException(FizzError.ERROR_REQUEST_FAILED, "session_creation_failed");
        private static readonly FizzException ERROR_INVALID_LOCALE = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_locale");

        readonly string _appId;
        readonly string _appSecret;
        readonly IFizzRestClient _restClient;

        public FizzIdSecretSessionProvider (string appId, string appSecret, IFizzRestClient restClient)
        {
            if (string.IsNullOrEmpty(appId))
            {
                throw FizzException.ERROR_INVALID_APP_ID;
            }

            if (string.IsNullOrEmpty (appSecret))
            {
                throw FizzException.ERROR_INVALID_APP_SECRET;
            }

            _appId = appId;
            _appSecret = appSecret;
            _restClient = restClient;
        }

        public void FetchToken(string userId, string locale, Action<FizzSession, FizzException> callback)
        {
            if (string.IsNullOrEmpty (locale))
            {
                FizzUtils.DoCallback<FizzSession>(new FizzSession(null, null, 0), ERROR_INVALID_LOCALE, callback);
                return;
            }

            JSONClass node = new JSONClass();
            node["user_id"] = userId;
            node["locale"] = locale;
            node["app_id"] = _appId;

            string body = node.ToString();
            string digest = GenerateHmac(body, _appSecret);
            var headers = new Dictionary<string, string>() { { "Authorization", "HMAC-SHA256 " + digest } };

            _restClient.Post(FizzConfig.API_BASE_URL, FizzConfig.API_PATH_SESSIONS, body, headers, (response, ex) =>
            {
                if (ex != null)
                {
                    FizzUtils.DoCallback (new FizzSession ("", "", FizzUtils.Now ()), ex,  callback);
                }
                else
                {
                    try
                    {
                        FizzSession session = ParseSession(JSONNode.Parse(response));
                        FizzUtils.DoCallback (session, null, callback);
                    }
                    catch (Exception responseEx)
                    {
                        FizzLogger.E(responseEx.Message);
                        FizzUtils.DoCallback (new FizzSession ("", "", FizzUtils.Now ()), ERROR_SESSION_CREATION_FAILED, callback);
                    }
                }
            });
        }

        private FizzSession ParseSession(JSONNode json) 
        {
            string token = json["token"];
            string subId = json["subscriber_id"];
            long now = 0;
            long.TryParse(json["now_ts"], out now);

            return new FizzSession(token, subId, now);
        }

        private string GenerateHmac(string json, string secretKey)
        {
            var encoding = new System.Text.UTF8Encoding();
            var body = encoding.GetBytes(json);
            var key = encoding.GetBytes(secretKey);

            using (var encoder = new HMACSHA256(key))
            {
                byte[] hashmessage = encoder.ComputeHash(body);
                return System.Convert.ToBase64String(hashmessage);
            }
        }
    }
}