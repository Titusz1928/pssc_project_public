using System;
using System.Text.Json.Serialization;

namespace Lab2.Domain.Models
{
    public class AwbOrderInfo
    {
        public OrderHeader OrderHeader { get; set; }
        public Price OrderPrice { get; set; }
        public DateTime OrderDate { get; set; }

        [JsonConstructor]  // Use this attribute to specify which constructor to use during deserialization
        public AwbOrderInfo(OrderHeader orderHeader, Price orderPrice, DateTime orderDate)
        {
            OrderHeader = orderHeader;
            OrderPrice = orderPrice;
            OrderDate = orderDate;
        }

        public override string ToString()
        {
            return $"AwbOrderInfo {{ OrderHeader: {OrderHeader}, OrderPrice: {OrderPrice}, OrderDate: {OrderDate:yyyy-MM-dd HH:mm:ss} }}";
        }
    }
    
}