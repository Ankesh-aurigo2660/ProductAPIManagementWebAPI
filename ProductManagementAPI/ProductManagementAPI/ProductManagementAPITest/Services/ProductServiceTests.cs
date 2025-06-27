using Microsoft.Extensions.Logging;
using Moq;
using ProductManagementAPI.Models.DTOs;
using ProductManagementAPI.Models.Entities;
using ProductManagementAPI.Repositories;
using ProductManagementAPI.Services;

namespace ProductManagementAPITest.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly Mock<IProductIdGenerator> _mockIdGenerator;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _mockIdGenerator = new Mock<IProductIdGenerator>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_mockRepository.Object, _mockIdGenerator.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 100001, Name = "Product 1", StockAvailable = 10 },
                new Product { Id = 100002, Name = "Product 2", StockAvailable = 20 }
            };
            _mockRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _service.GetAllProductsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Product 1", result.First().Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithExistingId_ReturnsProduct()
        {
            // Arrange
            var productId = 100001;
            var product = new Product { Id = productId, Name = "Test Product", StockAvailable = 10 };
            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.Id);
            Assert.Equal("Test Product", result.Name);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var productId = 100001;
            _mockRepository.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            // Act
            var result = await _service.GetProductByIdAsync(productId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateProductAsync_WithValidData_ReturnsCreatedProduct()
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
            var generatedId = 100003;
            var createdProduct = new Product
            {
                Id = generatedId,
                Name = createDto.Name,
                Price = createDto.Price,
                Category = createDto.Category,
                Brand = createDto.Brand,
                StockAvailable = createDto.StockAvailable
            };

            _mockIdGenerator.Setup(x => x.GenerateUniqueIdAsync()).ReturnsAsync(generatedId);
            _mockRepository.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync(createdProduct);

            // Act
            var result = await _service.CreateProductAsync(createDto);

            // Assert
            Assert.Equal(generatedId, result.Id);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Price, result.Price);
        }

        [Fact]
        public async Task DecrementStockAsync_WithValidQuantity_ReturnsUpdatedProduct()
        {
            // Arrange
            var productId = 100001;
            var quantity = 5;
            var updatedProduct = new Product { Id = productId, Name = "Test Product", StockAvailable = 45 };
            _mockRepository.Setup(x => x.UpdateStockAsync(productId, -quantity)).ReturnsAsync(updatedProduct);

            // Act
            var result = await _service.DecrementStockAsync(productId, quantity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(45, result.StockAvailable);
        }

        [Fact]
        public async Task DecrementStockAsync_WithInvalidQuantity_ThrowsArgumentException()
        {
            // Arrange
            var productId = 100001;
            var invalidQuantity = -1;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.DecrementStockAsync(productId, invalidQuantity));
        }

        [Fact]
        public async Task AddToStockAsync_WithValidQuantity_ReturnsUpdatedProduct()
        {
            // Arrange
            var productId = 100001;
            var quantity = 10;
            var updatedProduct = new Product { Id = productId, Name = "Test Product", StockAvailable = 60 };
            _mockRepository.Setup(x => x.UpdateStockAsync(productId, quantity)).ReturnsAsync(updatedProduct);

            // Act
            var result = await _service.AddToStockAsync(productId, quantity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(60, result.StockAvailable);
        }

        [Fact]
        public async Task AddToStockAsync_WithInvalidQuantity_ThrowsArgumentException()
        {
            // Arrange
            var productId = 100001;
            var invalidQuantity = 0;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _service.AddToStockAsync(productId, invalidQuantity));
        }
    }
}
