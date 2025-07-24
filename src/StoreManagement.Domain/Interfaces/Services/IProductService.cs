using StoreManagement.Domain.DTOs;

namespace StoreManagement.Domain.Interfaces.Services;

public interface IProductService
{
    Task<string> GetProductsByStoreCodeAsync(int storeCode);
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto);
    Task<ProductDto> PatchProductAsync(Guid id, PatchProductDto patchProductDto);
    Task DeleteProductAsync(Guid id);
    Task<bool> ProductExistsAsync(Guid id);
    Task<bool> StoreExistsByCodeAsync(int storeCode);
}
