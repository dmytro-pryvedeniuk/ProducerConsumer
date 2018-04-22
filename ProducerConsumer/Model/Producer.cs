using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer.Model
{
    public class Producer
    {
        AutoResetEvent _go = new AutoResetEvent(false);

        private IModelMessenger _messenger;
        private CancellationTokenSource _cancellation;
        private Queue<string> _queue;
        private object _lock = new object();

        public Producer(IModelMessenger messenger)
        {
            _messenger = messenger;
            _queue = new Queue<string>();
        }

        public void Start()
        {
            _cancellation = new CancellationTokenSource();
            _messenger.Send(new StateResult(State.Started));

            Task.Run(new Action(Work));
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    if (_cancellation.IsCancellationRequested) return;
                    var task = "Task " + i;
                    EnqueueText(task);
                }
            }
            finally
            {
                EnqueueText(null);
            }
        }

        private void EnqueueText(string text)
        {
            lock (_lock)
                _queue.Enqueue(text);
            _go.Set();
        }

        public void Stop()
        {
            lock (_lock)
                _queue.Clear();
            EnqueueText(null);
            if (_cancellation != null)
                _cancellation.Cancel();
        }

        private void Work()
        {
            while (true)
            {
                string task = null;
                lock (_lock)
                {
                    if (_queue.Count > 0)
                    {
                        task = _queue.Dequeue();
                        if (task == null)
                        {
                            _messenger.Send(new StateResult(State.Stopped));
                            return;
                        }
                    }
                }

                if (task != null)
                {
                    Thread.Sleep(1000);
                    _messenger.Send(new ContentResult(task));
                }
                else
                    _go.WaitOne();
            }
        }
    }
}