using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentAPI.API.Controllers;
using PaymentAPI.Application.DTOs;
using PaymentAPI.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestPaymentAPI
{
    public class CardValidationControllerTests
    {
        private readonly Mock<ICardValidation> _mockService;
        private readonly CardValidationController _controller;

        public CardValidationControllerTests()
        {
            _mockService = new Mock<ICardValidation>();
            _controller = new CardValidationController(_mockService.Object);
        }

        [Fact]
        public async Task ValidateCard_ReturnsOk_WithValidResponse()
        {
            // Arrange
            var request = new CardValidationRequest
            {
                CardNumber = "4111111111111111",
                CVV = 123,
                ExpiryMonth = 12,
                ExpiryYear = 2025
            };

            _mockService
                .Setup(s => s.ValidateCard(request.CardNumber!, request.CVV, request.ExpiryMonth, request.ExpiryYear))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.ValidateCard(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;

            Assert.True(response?.valid);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task ValidateCard_ReturnsOk_WithInvalidCard()
        {
            var request = new CardValidationRequest
            {
                CardNumber = "123456",
                CVV = 123,
                ExpiryMonth = 1,
                ExpiryYear = 2024
            };

            _mockService
                .Setup(s => s.ValidateCard(request.CardNumber!, request.CVV, request.ExpiryMonth, request.ExpiryYear))
                .ReturnsAsync(false);

            var result = await _controller.ValidateCard(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = okResult.Value as dynamic;

            Assert.False(response.valid);
        }
    }
}
