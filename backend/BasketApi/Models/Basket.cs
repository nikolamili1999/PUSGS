using LiteDB;
using System.Collections.Generic;

namespace BasketApi.Models
{
    public class Basket
    {
        [BsonId]
        public string Id { get; set; }
        public List<BasketItem> BasketItems { get; set; }
    }
}
