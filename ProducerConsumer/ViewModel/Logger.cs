using System;
using GalaSoft.MvvmLight.Messaging;
using ProducerConsumer.Model;

namespace ProducerConsumer.ViewModel
{
    class Logger : IModelMessenger
    {
        public void Register(object receiver, Action<TaskResult> action)
        {
            Messenger.Default.Register(receiver, action);
        }

        public void Send(TaskResult result)
        {
            Messenger.Default.Send(result);
        }
    }
}
