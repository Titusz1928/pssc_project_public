namespace Lab2.Domain.Models
{
    public class OrderHeader
    {
        public OrderId OrderId { get; set; }
        public string Name { get; }
        public string Address { get; }

        public OrderHeader(string name, string address, OrderId orderId)
        {
            Name = name;
            Address = address;
            OrderId = orderId;
        }

        // Implement ToString method for debugging purposes
        public override string ToString()
        {
            return $"OrderHeader {{ OrderId: {OrderId}, Name: {Name}, Address: {Address} }}";
        }
    }
}