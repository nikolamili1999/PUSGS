using AutoMapper;
using Backend.Infrastructure;
using Newtonsoft.Json;
using OrderApi.Dto;
using OrderApi.Interfaces;
using OrderApi.Models;
using ProductApi.Dto;
using ProductApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

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
                var price = 0;
                foreach (var orderDetailsDto in orderDto.OrderDetails)
                {
                    var orderDetails = _mapper.Map<OrderDetail>(orderDetailsDto);
                    try
                    {
                        using var client = new HttpClient();
                        var uri = _remoteServiceBaseUrl;
                        client.Timeout = TimeSpan.FromMinutes(30);
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _remoteServiceBaseUrl + $"/{orderDetails.ProductId}");
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        var responseString = await client.SendAsync(request);

                        var product = JsonConvert.DeserializeObject<ProductDto>(await responseString.Content.ReadAsStringAsync());
                        orderDetails.ProductName = product.Name;
                        orderDetails.ProductPrice = product.Price;
                        orderDetails.ProductSeller = product.Seller;
                    }catch(Exception ex) {
                        Console.WriteLine(ex.Message);
                    }

                    orderDetailsList.Add(orderDetails);
                }

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
    }
}
