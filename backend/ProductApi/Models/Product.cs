using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Seller { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public uint Quantity { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
