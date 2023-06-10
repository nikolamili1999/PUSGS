using ProductApi.Dto;
using ProductApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProductApi.Interfaces
{
    public interface IProductService
    {
        Product GetProduct(int id);
        Task<Product> AddProduct(string seller, CreateUpdateProductDto productDto);
        Product UpdateProduct(int id, CreateUpdateProductDto productDto);
        void DeleteProduct(int id);
        List<Product> GetProducts(); 
        void UpdateQuantity(int id, uint quantity);
    }
}
