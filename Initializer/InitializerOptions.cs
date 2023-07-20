namespace Initializer
{
    public class InitializerOptions
    {
        public string? LogFilePath { get; set; }


        public required string EventBusQueueName { get; set; }
    }
}
