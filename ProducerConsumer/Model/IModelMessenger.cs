using System;

namespace ProducerConsumer.Model
{
    public interface IModelMessenger
    {
        void Send(TaskResult result);
        void Register(object receiver, Action<TaskResult> action);
    }
}