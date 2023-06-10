using BasketApi.Dto;
using BasketApi.Interfaces;
using BasketApi.Models;
using LiteDB;
using System.Linq;

namespace BasketApi.Services
{
    public class BasketService : IBasketService
    {
        private LiteDatabase db = new LiteDatabase(@"Baskets.db");

        public Basket AddItemToBasket(string email, BasketItem item)
        {
            var baskets = db.GetCollection<Basket>("baskets");
            var basket = GetBasket(email);
            var basketItem = basket.BasketItems.Find(i => i.ProductId == item.ProductId);
            if(basketItem == null)
            {
                basket.BasketItems.Add(item);
            }
            else
            {
                basketItem.Quantity += item.Quantity;
            }
            baskets.Upsert(basket);

            return basket;
        }

        public Basket GetBasket(string email)
        {
            var baskets = db.GetCollection<Basket>("baskets");

            var result = baskets.Find(i => i.Id == email).ToList();
            if (result.Count == 0)
            {
                return new Basket()
                {
                    Id = email,
                    BasketItems = new System.Collections.Generic.List<BasketItem>()
                };
            }

            return result.First();
        }

        public Basket SetBasket(string email, BasketDto dto)
        {
            var baskets = db.GetCollection<Basket>("baskets");

            var basket = new Basket()
            {
                Id = email,
                BasketItems = dto.BasketItems
            };
            baskets.Upsert(email, basket);

            return basket;
        }
    }
}
