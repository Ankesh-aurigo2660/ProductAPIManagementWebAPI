using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductManagementAPI.Controllers;
using ProductManagementAPI.Models.DTOs;
using ProductManagementAPI.Models.Responses;
using ProductManagementAPI.Services;

namespace ProductManagementAPITest.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkWithProducts()
        {
            // Arrange
            var products = new List<ProductDTO>
            {
                new ProductDTO { Id = 100001, Name = "Test Product 1", StockAvailable = 10 },
                new ProductDTO { Id = 100002, Name = "Test Product 2", StockAvailable = 20 }
            };
            _mockProductService.Setup(x => x.GetAllProductsAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<IEnumerable<ProductDTO>>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(2, response.Data?.Count());
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            var productId = 100001;
            var product = new ProductDTO { Id = productId, Name = "Test Product", StockAvailable = 10 };
            _mockProductService.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(productId, response.Data?.Id);
        }

        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange
            var invalidId = 123; // Less than 6 digits

            // Act
            var result = await _controller.GetProduct(invalidId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(badRequestResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task GetProduct_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var productId = 100001;
            _mockProductService.Setup(x => x.GetProductByIdAsync(productId)).ReturnsAsync((ProductDTO?)null);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(notFoundResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task CreateProduct_WithValidData_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreateProductDTO
            {
                Name = "New Product",
                Price = 29.99m,
                Category = "Electronics",
                Brand = "TestBrand",
                StockAvailable = 50
            };
            var createdProduct = new ProductDTO
            {
                Id = 100003,
                Name = createDto.Name,
                Price = createDto.Price,
                Category = createDto.Category,
                Brand = createDto.Brand,
                StockAvailable = createDto.StockAvailable
            };
            _mockProductService.Setup(x => x.CreateProductAsync(createDto)).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.CreateProduct(createDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(createdResult.Value);
            Assert.True(response.Success);
            Assert.Equal(createdProduct.Id, response.Data?.Id);
        }

        [Fact]
        public async Task DecrementStock_WithValidData_ReturnsOk()
        {
            // Arrange
            var productId = 100001;
            var quantity = 5;
            var updatedProduct = new ProductDTO { Id = productId, Name = "Test Product", StockAvailable = 45 };
            _mockProductService.Setup(x => x.DecrementStockAsync(productId, quantity)).ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.DecrementStock(productId, quantity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(45, response.Data?.StockAvailable);
        }

        [Fact]
        public async Task DecrementStock_WithInvalidQuantity_ReturnsBadRequest()
        {
            // Arrange
            var productId = 100001;
            var invalidQuantity = -1;

            // Act
            var result = await _controller.DecrementStock(productId, invalidQuantity);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(badRequestResult.Value);
            Assert.False(response.Success);
        }

        [Fact]
        public async Task AddToStock_WithValidData_ReturnsOk()
        {
            // Arrange
            var productId = 100001;
            var quantity = 10;
            var updatedProduct = new ProductDTO { Id = productId, Name = "Test Product", StockAvailable = 60 };
            _mockProductService.Setup(x => x.AddToStockAsync(productId, quantity)).ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.AddToStock(productId, quantity);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<ApiResponse<ProductDTO>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(60, response.Data?.StockAvailable);
        }
    }
}
