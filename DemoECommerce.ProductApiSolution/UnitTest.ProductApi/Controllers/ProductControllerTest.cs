using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controllers
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productController;

        public ProductControllerTest()
        {
            // set up dependencies
            productInterface = A.Fake<IProduct>();

            // set up System under test
            productController = new ProductsController(productInterface);
        }

        // GET ALL PRODUCTS
        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnsOkResponseWithProducts()
        {
            // Arrange
            var products = new List<Product>()
            {
                new(){Id = 1, Name = "Product 1", Quantity = 5, Price = 10.0m },
                new(){Id = 2, Name = "Product 2", Quantity = 55, Price = 100.0m }
            };
            // Set up fake response for getall
            A.CallTo(() => productInterface.GetAllAsync())
                .Returns(products);

            // Act
            var result = await productController.GetProducts();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = okResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().Id.Should().Be(1);
            returnedProducts.Last().Id.Should().Be(2);
        }

        [Fact]
        public async Task GetProduct_WhenNoProductExists_ReturnNotFoundResponse()
        {
            // Arange
            var products = new List<Product>();

            // Set up fake response for getall
            A.CallTo(() => productInterface.GetAllAsync())
                .Returns(products);
            // Act
            var result = await productController.GetProducts();
            // Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }

        // CREATE PRODUCT
        [Fact]
        public async Task CreateProduct_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            productController.ModelState.AddModelError("Name", "Required");
            // Act
            var result = await productController.CreateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProduct_WhenCreateIsSuccesfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Created");
            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored))
                .Returns(response);
            var result = await productController.CreateProduct(productDTO);
            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Should().Be("Created");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProduct_WhenCreateFails_ReturnBasRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(false, "Failed");
            // Act
            A.CallTo(() => productInterface.CreateAsync(A<Product>.Ignored))
                .Returns(response);
            var result = await productController.CreateProduct(productDTO);
            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Should().Be("Failed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateisSuccesfull_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Updated");

            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored))
                .Returns(response);
            var result = await productController.UpdateProduct(productDTO);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Should().Be("Updated");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateFails_ReturnBadRequestResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(false, "Failed");

            // Act
            A.CallTo(() => productInterface.UpdateAsync(A<Product>.Ignored))
                .Returns(response);
            var result = await productController.UpdateProduct(productDTO);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Should().Be("Failed");
            responseResult!.Flag.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteIsSuccesful_ReturnOkResponse()
        {
            // Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(true, "Deleted");

            // set up 
            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored))
                .Returns(response);

            // act
            var result = await productController.DeleteProduct(productDTO);

            // assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
            var responseResult = okResult.Value as Response;
            responseResult!.Should().Be("Deleted");
            responseResult!.Flag.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteProduct_WhenDeleteFails_ReturnBadRequestResponse()
        {
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.95m);
            var response = new Response(false, "Failed");

            A.CallTo(() => productInterface.DeleteAsync(A<Product>.Ignored))
                .Returns(response);
            var result = await productController.DeleteProduct(productDTO);

            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Should().Be("Failed");
            responseResult!.Flag.Should().BeFalse();
        }
    }
}
