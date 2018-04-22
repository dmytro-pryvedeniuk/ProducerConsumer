namespace ProducerConsumer.Model
{
    public class ContentResult: TaskResult
    {
        public ContentResult(string output)
        {
            Output = output;
        }

        public string Output { get; private set; }
    }
}