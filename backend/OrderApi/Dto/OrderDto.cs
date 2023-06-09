using System;
using System.Collections.Generic;

namespace OrderApi.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public String Customer { get; set; }
        public List<String> Sellers { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; }
        public String Comment { get; set; }
        public String Address { get; set; }
        public long UTCTimeOrderCreated { get; set; }
        public long UTCTimeDeliveryStarted { get; set; }
        public long UTCTimeDeliveryExpected { get; set; }
    }
}
