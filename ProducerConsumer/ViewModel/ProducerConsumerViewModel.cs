using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ProducerConsumer.Model;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProducerConsumer.ViewModel
{
    public class ProducerConsumerViewModel : ViewModelBase
    {
        private Producer _producer;
        private IDispatcher _dispatcher;

        public ProducerConsumerViewModel(IModelMessenger logger, IDispatcher dispatcher)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            Results = new ObservableCollection<TaskResult>();
            _producer = new Producer(logger);
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            _isWorking = false;

            StartCommand = new RelayCommand(() =>
            {
                _isWorking = true;
                _producer.Start();
                UpdateCommands();
            }, () => { return !_isWorking; });

            StopCommand = new RelayCommand(() =>
            {
                _isWorking = false;
                _producer.Stop();
                UpdateCommands();
            }, () => { return _isWorking; });

            ClearCommand = new RelayCommand(() =>
            {
                Results.Clear();
                UpdateCommands();
            }, () => Results.Any());

            logger.Register(this, Work);
        }

        public void Work(TaskResult taskResult)
        {
            _dispatcher.Dispatch(() =>
            {
                if (taskResult is ContentResult)
                    Results.Add(taskResult);
                else if (taskResult is StateResult)
                    _isWorking = ((StateResult)taskResult).State == State.Started;
                UpdateCommands();
            });
        }

        private void UpdateCommands()
        {
            StartCommand.RaiseCanExecuteChanged();
            StopCommand.RaiseCanExecuteChanged();
            ClearCommand.RaiseCanExecuteChanged();
        }

        private bool _isWorking;

        public RelayCommand StartCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand ClearCommand { get; private set; }
        public ObservableCollection<TaskResult> Results { get; private set; }
    }
}