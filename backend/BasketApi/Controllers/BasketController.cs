using BasketApi.Dto;
using BasketApi.Interfaces;
using BasketApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BasketApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public BasketController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetBasket()
        {
            var email = GetUserEmail();
            try
            {
                return Ok(_basketService.GetBasket(email));
            }
            catch(Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult UpdateBasket(BasketDto dto)
        {
            var email = GetUserEmail();
            try
            {
                return Ok(_basketService.SetBasket(email, dto));
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPut]
        [Authorize]
        public ActionResult AddBasketItem(BasketItem item)
        {
            var email = GetUserEmail();
            try
            {
                return Ok(_basketService.AddItemToBasket(email, item));
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<ActionResult> Checkout(CheckoutDto dto)
        {
            var email = GetUserEmail();
            try
            {
                var basket = _basketService.GetBasket(email); 
                var _bearer_token = Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                // send message to order api

                var orderDto = await _orderService.CreateOrder(basket, dto, _bearer_token);
                
                return Ok(orderDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new {message = ex.Message});
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
