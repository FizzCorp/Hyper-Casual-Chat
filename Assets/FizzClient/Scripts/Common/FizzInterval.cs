using System;
using UnityEngine;

namespace Fizz.Common
{
	public class FizzInterval
    {
        bool _isEnabled = false;
        readonly IFizzActionDispatcher _dispatcher = null;
        readonly int _intervalMS;
        readonly Action _callback;

        public FizzInterval(IFizzActionDispatcher dispatcher, Action callback, int intervalMS)
        {
            if (dispatcher == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_dispatcher");
            }
            if (callback == null)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_interval_callback");
            }
            if (intervalMS <= 0)
            {
                throw new FizzException(FizzError.ERROR_BAD_ARGUMENT, "invalid_interval");
            }

            _dispatcher = dispatcher;
            _callback = callback;
            _intervalMS = intervalMS;
        }

        public void Enable()
        {
            if(_isEnabled)
            {
                return;
            }

            _isEnabled = true;
            _dispatcher.Delay(_intervalMS, Tick);
        }

        public void Disable()
        {
            _isEnabled = false;
        }

        public void Tick()
        {
            if (!_isEnabled)
            {
                return;
            }

            _callback.Invoke();
            _dispatcher.Delay(_intervalMS, Tick);
        }
    }
}