using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumer.Model
{
    public class BlockingProducer
    {
        AutoResetEvent _ready = new AutoResetEvent(false);
        AutoResetEvent _go = new AutoResetEvent(false);

        static volatile string _task;
        private IModelMessenger _messenger;
        private CancellationTokenSource _cancellation;

        public BlockingProducer(IModelMessenger messenger)
        {
            _messenger = messenger;
        }

        public void Start()
        {
            _cancellation = new CancellationTokenSource();
            Task.Run(() =>
            {
                _messenger.Send(new StateResult(State.Started));

                var t = new Thread(Work);
                t.Start();
                for (int i = 0; i < 3; i++)
                {
                    _ready.WaitOne();
                    if (_cancellation.IsCancellationRequested) return;
                    _task = "Task " + i;
                    _go.Set();
                }

                _ready.WaitOne();
                _task = null;
                _go.Set();

                _messenger.Send(new StateResult(State.Stopped));
            });
        }

        public void Stop()
        {
            if (_cancellation != null)
                _cancellation.Cancel();
        }

        private void Work()
        {
            while (true)
            {
                _ready.Set();
                _go.WaitOne();
                if (_task == null)
                    return;
                Thread.Sleep(1000);
                _messenger.Send(new ContentResult(_task));
            }
        }
    }
}