using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using OrderApi.Dto;
using OrderApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using OrderApi.Models;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: api/Order
        [HttpGet]
        [Authorize]
        public IActionResult GetOrders()
        {
                var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var role = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
            var email = GetUserEmail();
            var orders = _orderService.GetOrders();
            switch (role)
            {
                case "User":
                    orders = orders.FindAll(i => i.Customer.ToLower() == email.ToLower());
                    break;
                case "Seller":
                    orders = orders.FindAll(i => i.Sellers.Contains(email.ToLower()));
                    break;
                case "Admin":
                    break;
                default:
                    orders = new List<OrderDto>();
                    break;
            }
            return Ok(orders);
        }
        // GET: api/Order
        [HttpGet("waiting")]
        [Authorize(Roles = "Seller")]
        public IActionResult GetActiveOrders()
        {
            var userEmail = GetUserEmail();
            var orders = _orderService.GetOrders().FindAll(i => i.Sellers.Contains(userEmail.ToLower()) && i.UTCTimeDeliveryExpected > DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return Ok(orders);
        }

        // GET: api/Order/5
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetOrder(int id)
        {
            return Ok(_orderService.GetOrder(id));
        }

        // POST: api/Order
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> PostOrder(CreateOrderDto createOrderDto)
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var username = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
            try
            {
                return Ok(await _orderService.AddOrder(username, createOrderDto, _bearer_token));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id}/cancel")]
        [Authorize(Roles = "User")]
        public ActionResult CancelOrder(int id)
        {
            try
            {
                _orderService.CancelOrder(id);
                return Ok();
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        private string GetUserEmail()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
