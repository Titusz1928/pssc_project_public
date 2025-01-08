namespace Lab2.Domain.Events
{
    public class OrderEvent
    {
        public bool IsSuccessful { get; }
        public string Message { get; }

        public OrderEvent(bool isSuccessful, string message)
        {
            IsSuccessful = isSuccessful;
            Message = message;
        }
    }
}