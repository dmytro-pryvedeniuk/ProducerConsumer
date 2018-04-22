namespace ProducerConsumer.Model
{
    public class StateResult: TaskResult
    {
        public StateResult(State state)
        {
            State = state;
        }

        public State State { get; private set; }
    }
}