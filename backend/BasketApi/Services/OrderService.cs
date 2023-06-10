using System;
using BasketApi.Dto;
using BasketApi.Interfaces;
using BasketApi.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OrderApi.Dto;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BasketApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl = "https://localhost:5030/api/orders";

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<OrderDto> CreateOrder(Basket basket, CheckoutDto checkoutDto, string token)
        {
            var uri = _remoteServiceBaseUrl;
            var createOrderDto = new CreateOrderDto()
            {
                Comment = checkoutDto.Comment,
                Address = checkoutDto.Address,
                OrderDetails = basket.BasketItems.Select(x => new CreateOrderDetailDto()
                {
                    ProductId = x.ProductId,
                    Quantity = (uint)x.Quantity
                }).ToList(),
            };

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, _remoteServiceBaseUrl);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = new StringContent(JsonConvert.SerializeObject(createOrderDto),
                                                Encoding.UTF8,
                                                "application/json");

            var responseString = await _httpClient.SendAsync(request);
            if (responseString.IsSuccessStatusCode)
            {
                var order = JsonConvert.DeserializeObject<OrderDto>(await responseString.Content.ReadAsStringAsync());
                return order;
            }
            else
            {
                throw new Exception(await responseString.Content.ReadAsStringAsync());
            }

        }
    }
}
