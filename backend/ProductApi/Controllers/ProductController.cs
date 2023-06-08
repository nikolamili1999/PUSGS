﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ProductApi.Dto;
using ProductApi.Infrastructure;
using ProductApi.Interfaces;
using System;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IConfiguration _config;

        public ProductController(IProductService productService, IConfiguration configuration)
        {
            _productService = productService;
            _config = configuration;
        }

        [HttpGet("{id}")]
        public ActionResult GetProduct(int id)
        {
            var product = _productService.GetProduct(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [HttpGet]
        public ActionResult GetProducts()
        {
            return Ok(_productService.GetProducts());
        }

        //[HttpPut("{id}/quantity")]
        //public ActionResult UpdateProductQuantity(int id, )

        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<ActionResult> AddProductAsync([FromForm] CreateUpdateProductDto productDto)
        {
            var sellerEmail = GetUserEmail();
            return Ok(await _productService.AddProduct(sellerEmail, productDto));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public ActionResult UpdateProduct(int id, [FromForm] CreateUpdateProductDto updateProductDto)
        {
            var sellerEmail = GetUserEmail();
            var product = _productService.GetProduct(id);
            if(product.Seller != sellerEmail)
            {
                return Forbid("You cannot edit products that are not yours.");
            }
            return Ok(_productService.UpdateProduct(id, updateProductDto));
        }

        [HttpGet("{id}/image")]
        //[Authorize]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var path = Path.Combine(_config["StoredFilesPath"], id + ".png");
            //MemoryStream memory = new MemoryStream();
            //using (FileStream stream = new FileStream(path, FileMode.Open))
            //{
            //    await stream.CopyToAsync(memory);
            //}
            //memory.Position = 0;

            var image = System.IO.File.OpenRead(path);
            return File(image, "image/png");
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public ActionResult DeleteProduct(int id)
        {
            var sellerEmail = GetUserEmail();
            var product = _productService.GetProduct(id);
            if (product.Seller != sellerEmail)
            {
                return Forbid("You cannot edit products that are not yours.");
            }
            _productService.DeleteProduct(id);
            return Ok();
        }

        [NonAction]
        private string GetUserEmail()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            return claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
        }

    }
}