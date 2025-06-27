using Microsoft.AspNetCore.Mvc;
using ProductManagementAPI.Models.DTOs;
using ProductManagementAPI.Models.Responses;
using ProductManagementAPI.Services;

namespace ProductManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get All Products
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductDTO>>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(ApiResponse<IEnumerable<ProductDTO>>.SuccessResponse(products, "Products retrieved successfully"));
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> GetProduct(int id)
        {
            if (id < 100000 || id > 999999)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Invalid product ID format. ID must be 6 digits."));
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDTO>.ErrorResponse($"Product with ID {id} not found"));
            }

            return Ok(ApiResponse<ProductDTO>.SuccessResponse(product, "Product retrieved successfully"));
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        /// <param name="createProductDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> CreateProduct([FromBody] CreateProductDTO createProductDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Validation failed", errors));
            }

            try
            {
                var createdProduct = await _productService.CreateProductAsync(createProductDto);
                return CreatedAtAction(
                    nameof(GetProduct),
                    new { id = createdProduct.Id },
                    ApiResponse<ProductDTO>.SuccessResponse(createdProduct, "Product created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Failed to create product", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateProductDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> UpdateProduct(int id, [FromBody] UpdateProductDTO updateProductDto)
        {
            if (id < 100000 || id > 999999)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Invalid product ID format. ID must be 6 digits."));
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Validation failed", errors));
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);
            if (updatedProduct == null)
            {
                return NotFound(ApiResponse<ProductDTO>.ErrorResponse($"Product with ID {id} not found"));
            }

            return Ok(ApiResponse<ProductDTO>.SuccessResponse(updatedProduct, "Product updated successfully"));
        }

        /// <summary>
        /// Delete a product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
        {
            if (id < 100000 || id > 999999)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("Invalid product ID format. ID must be 6 digits."));
            }

            var result = await _productService.DeleteProductAsync(id);
            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"Product with ID {id} not found"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null, "Product deleted successfully"));
        }

        /// <summary>
        /// Decrement product stock
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("decrement-stock/{id}/{quantity}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> DecrementStock(int id, int quantity)
        {
            if (id < 100000 || id > 999999)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Invalid product ID format. ID must be 6 digits."));
            }

            if (quantity <= 0)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Quantity must be greater than 0"));
            }

            try
            {
                var product = await _productService.DecrementStockAsync(id, quantity);
                if (product == null)
                {
                    return NotFound(ApiResponse<ProductDTO>.ErrorResponse($"Product with ID {id} not found"));
                }

                return Ok(ApiResponse<ProductDTO>.SuccessResponse(product, $"Stock decremented by {quantity} successfully"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Add to product stock
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        [HttpPut("add-to-stock/{id}/{quantity}")]
        public async Task<ActionResult<ApiResponse<ProductDTO>>> AddToStock(int id, int quantity)
        {
            if (id < 100000 || id > 999999)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Invalid product ID format. ID must be 6 digits."));
            }

            if (quantity <= 0)
            {
                return BadRequest(ApiResponse<ProductDTO>.ErrorResponse("Quantity must be greater than 0"));
            }

            var product = await _productService.AddToStockAsync(id, quantity);
            if (product == null)
            {
                return NotFound(ApiResponse<ProductDTO>.ErrorResponse($"Product with ID {id} not found"));
            }

            return Ok(ApiResponse<ProductDTO>.SuccessResponse(product, $"Stock increased by {quantity} successfully"));
        }
    }
}
