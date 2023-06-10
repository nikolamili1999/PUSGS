using BasketApi.Dto;
using BasketApi.Models;
using OrderApi.Dto;
using System.Threading.Tasks;

namespace BasketApi.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrder(Basket basket, CheckoutDto checkoutDto, string token);
    }
}
