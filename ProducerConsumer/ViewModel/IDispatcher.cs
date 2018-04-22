using System;

namespace ProducerConsumer.ViewModel
{
    public interface IDispatcher
    {
        void Dispatch(Action action);
    }
}
