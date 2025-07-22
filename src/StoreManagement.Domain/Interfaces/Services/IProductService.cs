using StoreManagement.Domain.DTOs;

namespace StoreManagement.Domain.Interfaces.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductDto>> GetProductsByCompanyAsync(Guid companyId);
    Task<IEnumerable<ProductDto>> GetProductsByStoreAsync(Guid storeId);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
    Task DeleteProductAsync(Guid id);
    Task<bool> ProductExistsAsync(Guid id);
    Task<bool> CompanyExistsAsync(Guid companyId);
    Task<bool> StoreExistsAsync(Guid storeId);
    Task<string> GetProductsAsJsonAsync(Guid companyId);
}
