using AutoMapper;
using Backend.Infrastructure;
using OrderApi.Dto;
using OrderApi.Interfaces;
using OrderApi.Models;
using ProductApi.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OrderApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IMapper _mapper;
        private readonly OrdersDbContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly string _remoteServiceBaseUrl = "https://localhost:5010/api/product";
        private static Object thisLock = new Object();
        private static Object addLock = new object();
        private static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);
        private Random _random = new Random();

        public OrderService(IMapper mapper, OrdersDbContext dbContext, HttpClient httpClient)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _httpClient = httpClient;
        }
        public async Task<OrderDto> AddOrder(string customerEmail, CreateOrderDto orderDto, string token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                List<OrderDetail> orderDetailsList = new List<OrderDetail>();
                var quantitiesUpdate = new List<UpdateQuantityDto>();
                var price = 0;
                foreach (var orderDetailsDto in orderDto.OrderDetails)
                {
                    var orderDetails = _mapper.Map<OrderDetail>(orderDetailsDto);
                    var product = await GetProduct(orderDetails.ProductId, token);

                    orderDetails.ProductName = product.Name;
                    orderDetails.ProductPrice = product.Price;
                    orderDetails.ProductSeller = product.Seller;
                    if (orderDetails.Quantity > product.Quantity)
                    {
                        throw new Exception($"Quantity is exceded for product {product.Name}");
                    }
                    quantitiesUpdate.Add(new UpdateQuantityDto()
                    {
                        // new quantity
                        Quantity = product.Quantity - orderDetails.Quantity,
                        ProductId = orderDetails.ProductId,
                    });
                    orderDetailsList.Add(orderDetails);
                }
                // update products quntities
                await UpdateProductQuantitiesAsync(quantitiesUpdate, token);

                var order = _mapper.Map<Order>(orderDto);
                order.OrderDetails = null;
                order.Customer = customerEmail;
                order.UTCTimeOrderCreated = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                order.UTCTimeDeliveryExpected = DateTimeOffset.Now.AddMinutes(_random.Next(20,60)).ToUnixTimeMilliseconds();
                _dbContext.Add(order);
                await _dbContext.SaveChangesAsync();
                foreach (var orderDetail in orderDetailsList)
                    orderDetail.OrderId = order.Id;
                await _dbContext.OrderDetails.AddRangeAsync(orderDetailsList);
                await _dbContext.SaveChangesAsync();

                return _mapper.Map<OrderDto>(order);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task<ProductDto> GetProduct(int id, string token)
        {
            using var client = new HttpClient();
            var uri = _remoteServiceBaseUrl;
            client.Timeout = TimeSpan.FromMinutes(30);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _remoteServiceBaseUrl + $"/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var responseString = await client.SendAsync(request);
            var json = await responseString.Content.ReadAsStringAsync();
            var product = JsonConvert.DeserializeObject<ProductDto>(json);
            return product;
        }

        /// <summary>
        /// Updates product quantities
        /// </summary>
        /// <param name="updateQuantities">Object that holds new quantity values</param>
        /// <param name="token">Auhtorization bearer token from user</param>
        /// <returns></returns>
        /// <exception cref="Exception">Throws exception if request status code is not successfull</exception>
        private async Task UpdateProductQuantitiesAsync(List<UpdateQuantityDto> updateQuantities, String token)
        {
            using var client = new HttpClient();
            var uri = _remoteServiceBaseUrl;
            client.Timeout = TimeSpan.FromMinutes(30);
            var bodyJson = JsonConvert.SerializeObject(updateQuantities);
            var stringContent = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Patch, _remoteServiceBaseUrl + "/quantity");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error occured during order creating. Try again later.");
            }
        }

        public OrderDto GetOrder(int id)
        {
            return _mapper.Map<OrderDto>(_dbContext.Orders.Find(id));
        }

        public List<OrderDto> GetOrders()
        {
            var orders = _dbContext.Orders.ToList();
            orders.ForEach(order =>
            {
                order.OrderDetails = _dbContext.OrderDetails.ToList().FindAll(i => i.Order.Id == order.Id);
            });
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public List<OrderDto> GetOrdersByUser(string userEmail)
        {
            return _mapper.Map<List<OrderDto>>(_dbContext.Orders.ToList().FindAll(i => i.Customer == userEmail.ToLower()));
        }

        public List<OrderDto> GetPendingOrders()
        {
            return _mapper.Map<List<OrderDto>>(_dbContext.Orders.ToList().FindAll(i => i.UTCTimeDeliveryExpected == 0).ToList());
        }

        public List<OrderDto> GetOrdersBySeller(string sellerEmail)
        {
            return _mapper.Map<List<OrderDto>>(_dbContext.Orders.ToList().FindAll(i => i.Sellers.Contains(sellerEmail.ToLower())));
        }

        public async Task CancelOrder(int id, string token)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                var order = _dbContext.Orders.Find(id);
                if(order.TimeDeliveryExpected > DateTime.Now)
                {
                    throw new Exception("Order is delivered");
                }
                var quantitiesUpdate = new List<UpdateQuantityDto>();
                foreach(var orderDetails in order.OrderDetails)
                {
                    var product = await GetProduct(orderDetails.ProductId, token);
                    quantitiesUpdate.Add(new UpdateQuantityDto()
                    {
                        ProductId = orderDetails.ProductId,
                        Quantity = orderDetails.Quantity + product.Quantity,
                    });
                }
                // update products quntities
                // return it to previous state
                await UpdateProductQuantitiesAsync(quantitiesUpdate, token);
                order.IsCancelled = true;
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
    }
}
