namespace OrderApi.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductSeller { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public uint Quantity { get; set; }
        public double ProductPrice { get; set; }
        public string ProductName { get; set; }
        public double Price { get {return ProductPrice * Quantity; } }
    }
}
