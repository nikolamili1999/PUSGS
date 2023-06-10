using System.Collections.Generic;

namespace ProductApi.Dto
{
    public class UpdateQuantityDto
    {
        public int ProductId { get; set; }
        public uint Quantity { get; set; }
    }
}
