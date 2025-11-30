using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OrderApi.Application.DTOs;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using OrderApi.Domain.Entities;
using System.Linq.Expressions;
using System.Net.Http.Json;

namespace UnitTest.OrderApi.Services
{
    public class OrderServiceTest
    {
        private readonly IOrderService orderServiceInterface;
        private readonly IOrder orderInterface;

        public OrderServiceTest() {
            orderServiceInterface = A.Fake<IOrderService>();
            orderInterface = A.Fake<IOrder>();
        }
        // CREATE FAKE HHTTP MESSAGE HANDLER
        public class FakeHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
        {
            private readonly HttpResponseMessage _response = response;

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
        }

        // CREATE FAKE HTTP CLIENT USING FAKE HTTP MESSAGE HANDLER
        private static HttpClient CreateFakeHttpClient(object o)
        {
            var httpResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(o)
            };
            var fakeHttpMessageHandler = new FakeHttpMessageHandler(httpResponseMessage);
            var _httpClient = new HttpClient(fakeHttpMessageHandler)
            {
                BaseAddress = new Uri("http://localhost")
            };
            return _httpClient;
        }

        [Fact]
        public async Task GetProduct_ValidProductId_ReturnProduct()
        {
            // arranger
            int productId = 1;
            var productDTO = new ProductDTO(1, "Product 1", 13, 56.78m);
            var _httpClient = CreateFakeHttpClient(productDTO);

            // System Under Test - SUT
            // We only the httpclient to make calls
            // specify only httpclient and null to the rest
            var _orderService = new OrderService(null!, _httpClient, null!);

            // act
            var result = await _orderService.GetProduct(productId);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().Be(productId);
            result.Name.Should().Be("Product 1");
        }

        [Fact]
        public async Task GetProduct_InvalidProductId_ReturnNull()
        {
            int productId = 1;
            var _httpClient = CreateFakeHttpClient(null!);
            var _orderService = new OrderService(null!, _httpClient, null!);

            // act
            var result = await _orderService.GetProduct(productId);
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersByClintIdIs_OrderExist_ReturnOrderDetails()
        {
            // arrange
            int clientId = 1;
            var orders = new List<Order>
            {
                new() { Id = 1, ProductId = 1, ClientId = clientId, PurchaseQuantity = 2, OrderDate =  DateTime.Now },
                new() { Id = 1, ProductId =2, ClientId = clientId, PurchaseQuantity = 1, OrderDate = DateTime.Now  }
            };

            // mock getorderby
            A.CallTo(() => orderInterface.GetOrdersAsync(A<Expression<Func<Order, bool>>>.Ignored)).Returns(orders);
            var _orderServcie = new OrderService(orderInterface!, null!, null!);
            // act
            var result = await _orderServcie.GetOrderByClientId(clientId);

            // aseet
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().HaveCountGreaterThanOrEqualTo(2);
        }
    }
}
