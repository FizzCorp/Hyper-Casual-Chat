using System;

namespace Fizz.Common
{
    public class FizzConfig
    {
#if UNITY_2018_3_OR_NEWER && NET_4_6
        public static readonly string API_PROTOCOL = "https";
        public static readonly string API_ENDPOINT = "api.fizz.io";

        public static bool MQTT_USE_TLS = true;
        public static string MQTT_HOST_ENDPOINT = "mqtt.fizz.io";
#else
        public static readonly string API_PROTOCOL = "http";
        public static readonly string API_ENDPOINT = "api.fizz.io:3000";

        public static bool MQTT_USE_TLS = false;
        public static string MQTT_HOST_ENDPOINT = "mqtt.fizz.io";
#endif

        public static readonly string API_VERSION = "v1";
        public static readonly string API_BASE_URL = string.Format("{0}://{1}/{2}", API_PROTOCOL, API_ENDPOINT, API_VERSION);
        public static readonly string API_PATH_SESSIONS = "/sessions";
        public static readonly string API_PATH_EVENTS = "/events";
        public static readonly string API_PATH_REPORTS = "/reports";
        public static readonly string API_PATH_CONTENT_MODERATION = "/moderatedTexts";
        public static readonly string API_PATH_MESSAGES = "/channels/{0}/messages";
        public static readonly string API_PATH_SUBSCRIBERS = "/channels/{0}/subscribers";
        public static readonly string API_PATH_MESSAGE_ACTION = "/channels/{0}/messages/{1}";
        public static readonly string API_PATH_BAN = "/channels/{0}/bans";
        public static readonly string API_PATH_MUTE = "/channels/{0}/mutes";
        public static readonly string API_HEADER_SESSION_TOKEN = "Session-Token"; 
    }
}
