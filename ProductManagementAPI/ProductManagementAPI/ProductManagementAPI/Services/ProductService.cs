using ProductManagementAPI.Models.DTOs;
using ProductManagementAPI.Models.Entities;
using ProductManagementAPI.Repositories;

namespace ProductManagementAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductIdGenerator _productIdGenerator;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository, IProductIdGenerator productIdGenerator, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _productIdGenerator = productIdGenerator;
            _logger = logger;
        }
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select(MapToDto);
        }
        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            return product != null ? MapToDto(product) : null;
        }
        public async Task<ProductDTO> CreateProductAsync(CreateProductDTO createProductDto)
        {
            var productId = await _productIdGenerator.GenerateUniqueIdAsync();
            var product = new Product
            {
                Id = productId,
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                Category = createProductDto.Category,
                Brand = createProductDto.Brand,
                StockAvailable = createProductDto.StockAvailable,
                SKU = createProductDto.SKU,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            var createdProduct = await _productRepository.CreateAsync(product);
            _logger.LogInformation($"Created product with ID: {createdProduct.Id}");

            return MapToDto(createdProduct);
        }
        public async Task<ProductDTO?> UpdateProductAsync(int id, UpdateProductDTO updateProductDto)
        {
            var existingProduct = await _productRepository.GetByIdAsync(id);
            if (existingProduct == null)
            {
                return null;
            }

            existingProduct.Name = updateProductDto.Name;
            existingProduct.Description = updateProductDto.Description;
            existingProduct.Price = updateProductDto.Price;
            existingProduct.Category = updateProductDto.Category;
            existingProduct.Brand = updateProductDto.Brand;
            existingProduct.StockAvailable = updateProductDto.StockAvailable;
            existingProduct.SKU = updateProductDto.SKU;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            _logger.LogInformation($"Updated product with ID: {updatedProduct.Id}");

            return MapToDto(updatedProduct);
        }
        public async Task<bool> DeleteProductAsync(int id)
        {
            var result = await _productRepository.DeleteAsync(id);
            if (result)
            {
                _logger.LogInformation($"Deleted product with ID: {id}");
            }
            return result;
        }

        public async Task<ProductDTO?> DecrementStockAsync(int id, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            var product = await _productRepository.UpdateStockAsync(id, -quantity);
            if (product != null)
            {
                _logger.LogInformation($"Decremented stock for product {id} by {quantity}");
            }
            return product != null ? MapToDto(product) : null;
        }

        public async Task<ProductDTO?> AddToStockAsync(int id, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be positive", nameof(quantity));

            var product = await _productRepository.UpdateStockAsync(id, quantity);
            if (product != null)
            {
                _logger.LogInformation($"Added {quantity} to stock for product {id}");
            }
            return product != null ? MapToDto(product) : null;
        }

        private static ProductDTO MapToDto(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category,
                Brand = product.Brand,
                StockAvailable = product.StockAvailable,
                SKU = product.SKU,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

    }
}
