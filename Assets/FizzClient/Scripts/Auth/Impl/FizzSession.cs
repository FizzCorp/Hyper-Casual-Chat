namespace Fizz.Common
{
    public struct FizzSession
    {
        public readonly string _token;
        public readonly string _subscriberId;
        public readonly long _serverTS;

        public FizzSession(string token, string subscriberId, long serverTS)
        {
            _token = token;
            _subscriberId = subscriberId;
            _serverTS = serverTS;
        }
    }
}