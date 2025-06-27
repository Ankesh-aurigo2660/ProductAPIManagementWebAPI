using ProductManagementAPI.Data;

namespace ProductManagementAPI.Services
{
    public class ProductIdGenerator : IProductIdGenerator
    {
        private readonly ProductDBContext _productDBContext;
        private readonly ILogger<ProductIdGenerator> _logger;
        private static readonly object _lock = new object();
        public ProductIdGenerator(ProductDBContext productDBContext, ILogger<ProductIdGenerator> logger)
        {
            _productDBContext = productDBContext;
            _logger = logger;
        }
        public async Task<int> GenerateUniqueIdAsync()
        {
            lock (_lock)
            {
                try
                {
                    var maxAttempts = 100;
                    var attempts = 0;

                    while (attempts < maxAttempts)
                    {
                        var id = GenerateRandomId();

                        // Check if ID already exists
                        var exists = _productDBContext.Products.Any(p => p.Id == id);
                        if (!exists)
                        {
                            _logger.LogInformation($"Generated unique product ID: {id}");
                            return id;
                        }

                        attempts++;
                        _logger.LogWarning($"ID collision detected for {id}, attempting again. Attempt {attempts}");
                    }

                    throw new InvalidOperationException("Unable to generate unique ID after maximum attempts");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating unique product ID");
                    throw;
                }
            }
        }

        private int GenerateRandomId()
        {
            // Generate 6-digit ID (100000-999999)
            // Using timestamp + random for better distribution in multi-instance scenarios
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var random = new Random((int)(timestamp % int.MaxValue));

            return random.Next(100000, 1000000);
        }
    }
}
