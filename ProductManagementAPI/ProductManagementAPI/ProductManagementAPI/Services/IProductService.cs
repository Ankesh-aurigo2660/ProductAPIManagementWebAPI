using ProductManagementAPI.Models.DTOs;

namespace ProductManagementAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<ProductDTO> CreateProductAsync(CreateProductDTO createProductDto);
        Task<ProductDTO?> UpdateProductAsync(int id, UpdateProductDTO updateProductDto);
        Task<bool> DeleteProductAsync(int id);
        Task<ProductDTO?> DecrementStockAsync(int id, int quantity);
        Task<ProductDTO?> AddToStockAsync(int id, int quantity);
    }
}
