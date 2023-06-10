using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ProductApi.Dto;
using ProductApi.Infrastructure;
using ProductApi.Interfaces;
using ProductApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProductApi.Services
{
    public class ProductService: IProductService
    {
        private readonly IMapper _mapper;
        private readonly ProductsDbContext _dbContext;
        private readonly IConfiguration _config;
        private static Object thisLock = new Object();

        public ProductService(IMapper mapper, ProductsDbContext dbContext, IConfiguration configuration)
        {
            _mapper = mapper;
            _dbContext = dbContext;
            _config = configuration;
        }

        public async Task<Product> AddProduct(string seller, CreateUpdateProductDto createProductDto)
        {
            if (!createProductDto.ImageFile.ContentType.Contains("image"))
            {
                throw new Exception("File is not image");
            }
            var product = _mapper.Map<Product>(createProductDto);
            product.Seller = seller;
            _dbContext.Products.Add(product);
            _dbContext.SaveChanges();
            // save image with new product id name
            await SavePostImageAsync(createProductDto.ImageFile, product.Id);

            return product;
        }

        public async Task SavePostImageAsync(IFormFile formFile, int id)
        {
            var filePath = Path.Combine(_config["StoredFilesPath"], id.ToString() + ".png");

            using (var stream = System.IO.File.Create(filePath))
            {
                await formFile.CopyToAsync(stream);
            }
            return;
        }

        public void DeleteProduct(int id)
        {
            lock (thisLock)
            {
                var product = _dbContext.Products.Find(id);
                if (product == null) return;
                //_dbContext.Products.Remove(product);
                product.IsDeleted = true;
                _dbContext.SaveChanges();
            }
        }

        public Product GetProduct(int id)
        {
            return _dbContext.Products.Find(id);
        }

        public List<Product> GetProducts()
        {
            return _dbContext.Products.ToList().FindAll(i => i.IsDeleted == false);
        }

        public Product UpdateProduct(int id, CreateUpdateProductDto productDto)
        {
            lock (thisLock)
            {
                var product = _dbContext.Products.Find(id);
                _mapper.Map<CreateUpdateProductDto, Product>(productDto, product);
                _dbContext.SaveChanges();
                return product;
            }
        }

        public void UpdateQuantity(int id, uint quantity)
        {
            lock (thisLock)
            {
                var product = _dbContext.Products.Find(id);
                product.Quantity = quantity;
                _dbContext.SaveChanges();
            }
        }
    }
}
