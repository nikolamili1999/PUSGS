using Microsoft.AspNetCore.Http;

namespace ProductApi.Dto
{
    public class CreateUpdateProductDto
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public uint Quantity { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
