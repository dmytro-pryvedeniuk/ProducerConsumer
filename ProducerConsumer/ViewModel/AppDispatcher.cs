using System;
using System.Windows;

namespace ProducerConsumer.ViewModel
{
    public class AppDispatcher : IDispatcher
    {
        public void Dispatch(Action action)
        {
            Application.Current?.Dispatcher?.BeginInvoke(action);
        }
    }
}
