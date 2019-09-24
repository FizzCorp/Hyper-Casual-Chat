using System;

namespace Fizz
{
	public class FizzException: Exception
    {
        public static readonly FizzException ERROR_INVALID_APP_ID = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_app_id");
        public static readonly FizzException ERROR_INVALID_APP_SECRET = new FizzException (FizzError.ERROR_BAD_ARGUMENT, "invalid_app_secret");
        public static readonly FizzException ERROR_INVALID_USER_ID = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_user_id");
        public static readonly FizzException ERROR_INVALID_SESSION_REPOSITORY = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_session_repository");
        public static readonly FizzException ERROR_INVALID_EVENTBUS = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_eventbus");
        public static readonly FizzException ERROR_INVALID_MESSAGE_TYPE = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_message_type");
        public static readonly FizzException ERROR_INVALID_MESSAGE_DATA = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_message_data");
        public static readonly FizzException ERROR_INVALID_SUBSCRIBER_ID = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_subscriber_id");
        public static readonly FizzException ERROR_INVALID_SESSION_TOKEN = new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_session_token");

        public FizzException(int code, string reason): base(reason)
        {
            Code = code;
            Reason = reason;
        }

        public int Code { get; private set; }

        public string Reason { get; private set; }
    }
}
