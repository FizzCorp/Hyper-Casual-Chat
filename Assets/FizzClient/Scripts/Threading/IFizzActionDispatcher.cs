using System;

namespace Fizz.Common
{
    public interface IFizzActionDispatcher
    {
        void Post(Action action);
        void Delay(int delayMS, Action action);
    }
}
