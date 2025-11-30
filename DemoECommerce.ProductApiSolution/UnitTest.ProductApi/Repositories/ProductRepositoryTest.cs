using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ProductApi.Domain.Entities;
using ProductApi.infrastructure.Data;
using ProductApi.infrastructure.Repositories;
using System.Linq.Expressions;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDBContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDBContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb")
                .Options;
            productDbContext = new ProductDBContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        // CREATE PRODUCT
        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
        {
            // arrange
            var existingProduct = new Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // act
            var result = await productRepository.CreateAsync(existingProduct);

            // assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnSuccessResponse()
        {
            // arange
            var product = new Product() { Name = "New Product" };

            // act
            var result = await productRepository.CreateAsync(product);

            // assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnSuccessResponse()
        {
            // arange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 64.67m, Quantity = 5 };
            productDbContext.Products.Add(product);
            // act
            var result = await productRepository.DeleteAsync(product);
            // assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsNotFound_ReturnNotFoundResponse()
        {
            // arranmge
            var product = new Product() { Id = 2, Name = "NONExisting Product", Price = 64.67m, Quantity = 5 };

            // act
            var result = await productRepository.DeleteAsync(product);

            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnProduct()
        {
            // arange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 64.67m, Quantity = 5 };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // act
            var result = await productRepository.FindByIdAsync(product.Id);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Existing Product");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFOund_ReturnNull()
        {
            // arrange

            // act
            var result = await productRepository.FindByIdAsync(999);

            // assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        {
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Price = 10.0m, Quantity = 5 },
                new Product { Name = "Product 2", Price = 20.0m, Quantity = 10 }
            };
            productDbContext.Products.AddRange(products);
            await productDbContext.SaveChangesAsync();

            // act
            var result = await productRepository.GetAllAsync();
            // assert
            result.Should().NotBeNull();
            result.Count().Should().Be(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact]
        public async Task GetAllAsync_WhenProductsAreNotFound_ReturnEmptyList()
        {
            // arrange
            // act
            var result = await productRepository.GetAllAsync();
            // assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            // arrange
            var product = new Product() { Id = 1, Name = "Product 1"};
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 1";

            // act
            var result = await productRepository.GetByAsync(predicate);
            // assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        {
            // arrange

            Expression<Func<Product, bool>> predicate = p => p.Name == "Product 1";

            // act
            var result = await productRepository.GetByAsync(predicate);
            // assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsUpdated_ReturnSuccess()
        {
            var product = new Product() { Id = 1, Name = "Product 1"};
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            // act
            var result = await productRepository.UpdateAsync(product);

            // assert
            result.Should().NotBeNull();
            result.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_WhenProductIsNotFound_ReturnErrorResponse()
        {
            var product = new Product() { Id = 999, Name = "Non Existing Product" };
            // act
            var result = await productRepository.UpdateAsync(product);
            // assert
            result.Should().NotBeNull();
            result.Flag.Should().BeFalse();
        }
    }
}
