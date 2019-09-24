using System;

namespace Fizz.Common
{
    public class FizzTimer
    {
        private readonly object _synclock = new object();
        private bool _timeout = false;
        private bool _aborted = false;

        public FizzTimer(int delayMS, IFizzActionDispatcher dispatcher, Action onTimeout)
        {
            dispatcher.Delay(delayMS, () =>
            {
                lock (_synclock)
                {
                    if (!_aborted)
                    {
                        _timeout = true;
                        onTimeout();
                    }
                }
            });
        }

        public bool TryAbort()
        {
            lock (_synclock)
            {
                if (!_timeout)
                {
                    _aborted = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
