using System;

namespace SharedModels
{
    public class IOrder
    {
        public Guid OrderId { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
    }
}
