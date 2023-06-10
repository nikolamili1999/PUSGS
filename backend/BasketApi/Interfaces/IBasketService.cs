using BasketApi.Dto;
using BasketApi.Models;

namespace BasketApi.Interfaces
{
    public interface IBasketService
    {
        public Basket GetBasket(string email);
        public Basket SetBasket(string email, BasketDto dto);
        public Basket AddItemToBasket(string email, BasketItem item);
    }
}
