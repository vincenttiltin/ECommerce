using AutoMapper;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using ECommerce.Api.Products.Providers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ECommerce.Api.Products.Tests
{
    public class ProductsServiceTest
    {
        [Fact]
        public async Task GetProductsReturnAllProductsAsync()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().
                UseInMemoryDatabase(nameof(GetProductsReturnAllProductsAsync))
                .Options;

            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);
            var productsProvider = new ProductsProvider(dbContext,null,mapper);

            var products = await productsProvider.GetProductsAsync();

            Assert.True(products.IsSuccess);
            Assert.True(products.Products.Any());
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductUsingValidID()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().
                UseInMemoryDatabase(nameof(GetProductReturnsProductUsingValidID))
                .Options;

            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);
            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var products = await productsProvider.GetProductAsync(1);

            Assert.True(products.IsSuccess);
            Assert.NotNull(products.Product);
            Assert.True(products.Product.Id == 1);
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductUsingInvalidID()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().
                UseInMemoryDatabase(nameof(GetProductReturnsProductUsingInvalidID))
                .Options;

            var dbContext = new ProductsDbContext(options);
            CreateProducts(dbContext);

            var productProfile = new ProductProfile();
            var config = new MapperConfiguration(cfg => cfg.AddProfile(productProfile));
            var mapper = new Mapper(config);
            var productsProvider = new ProductsProvider(dbContext, null, mapper);

            var products = await productsProvider.GetProductAsync(-1);

            Assert.False(products.IsSuccess);
            Assert.Null(products.Product);
            Assert.NotNull(products.ErrorMessage);
        }



        private void CreateProducts(ProductsDbContext dbContext)
        {
            for(int i = 0; i < 10; i++)
            {
                dbContext.Products.Add(new Product()
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }
            dbContext.SaveChanges();
        }
    }
}
