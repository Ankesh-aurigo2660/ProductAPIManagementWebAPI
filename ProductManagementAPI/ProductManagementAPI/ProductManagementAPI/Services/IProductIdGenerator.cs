namespace ProductManagementAPI.Services
{
    public interface IProductIdGenerator
    {
        Task<int> GenerateUniqueIdAsync();
    }
}
