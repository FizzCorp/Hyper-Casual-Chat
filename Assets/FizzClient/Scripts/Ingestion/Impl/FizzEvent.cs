using Fizz.Common.Json;

namespace Fizz.Ingestion.Impl
{
    public class FizzEvent
    {
        private static long idCounter = 0;

        public FizzEvent(
            string userId,
            FizzEventType type,
            int ver,
            string sessionId,
            long time,
            string platform,
            string build,
            string custom01,
            string custom02,
            string custom03,
            JSONNode fields
        )
        {
            if (userId == null)
            {
                throw FizzException.ERROR_INVALID_USER_ID;    
            }
            if (sessionId == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_session_id");
            }

            Id = ++idCounter;
            UserId = userId;
            Type = type;
            Version = ver;
            SessionId = sessionId;
            Time = time;
            Platform = platform;
            Build = build;
            Custom01 = custom01;
            Custom02 = custom02;
            Custom03 = custom03;
            Fields = fields;
        }

        public long Id { get; private set; }
        public string UserId { get; private set; }
        public FizzEventType Type { get; private set; }
        public int Version { get; private set; }
        public string SessionId { get; private set; }
        public long Time { get; private set; }
        public string Platform { get; private set; }
        public string Build { get; private set; }
        public string Custom01 { get; private set; }
        public string Custom02 { get; private set; }
        public string Custom03 { get; private set; }
        public JSONNode Fields { get; private set; }
    }
}
