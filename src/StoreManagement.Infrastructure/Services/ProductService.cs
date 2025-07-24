using AutoMapper;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        IStoreRepository storeRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _storeRepository = storeRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<string> GetProductsByStoreCodeAsync(int storeCode)
    {
        var store = await _storeRepository.GetByCodeAsync(storeCode);
        if (store == null)
        {
            throw new ArgumentException($"Store with code {storeCode} not found.");
        }

        // Usar a scalar function para obter produtos ativos como JSON
        var jsonResult = await _productRepository.GetProductsAsJsonByStoreIdAsync(store.Id);

        return jsonResult;
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        // Validate store exists by code
        var store = await _storeRepository.GetByCodeAsync(createProductDto.StoreCode);
        if (store == null)
        {
            throw new ArgumentException($"Store with code {createProductDto.StoreCode} not found.");
        }

        var product = new Product
        {
            Name = createProductDto.Name,
            Code = createProductDto.Code,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            IsActive = createProductDto.IsActive,
            StoreId = store.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException($"Product with ID {id} not found.");

        // Validate store exists by code
        var store = await _storeRepository.GetByCodeAsync(updateProductDto.StoreCode);
        if (store == null)
        {
            throw new ArgumentException($"Store with code {updateProductDto.StoreCode} not found.");
        }

        product.Name = updateProductDto.Name;
        product.Code = updateProductDto.Code;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.IsActive = updateProductDto.IsActive;
        product.StoreId = store.Id;
        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.UpdateAsync(product);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> PatchProductAsync(Guid id, PatchProductDto patchProductDto)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            throw new ArgumentException($"Product with ID {id} not found.");

        // Update only provided fields
        if (patchProductDto.Name != null)
            product.Name = patchProductDto.Name;

        if (patchProductDto.Code.HasValue)
            product.Code = patchProductDto.Code.Value;

        if (patchProductDto.Description != null)
            product.Description = patchProductDto.Description;

        if (patchProductDto.Price.HasValue)
            product.Price = patchProductDto.Price.Value;

        if (patchProductDto.IsActive.HasValue)
            product.IsActive = patchProductDto.IsActive.Value;

        if (patchProductDto.StoreCode.HasValue)
        {
            var store = await _storeRepository.GetByCodeAsync(patchProductDto.StoreCode.Value);
            if (store == null)
            {
                throw new ArgumentException($"Store with code {patchProductDto.StoreCode.Value} not found.");
            }
            product.StoreId = store.Id;
        }

        product.UpdatedAt = DateTime.UtcNow;

        _productRepository.UpdateAsync(product);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task DeleteProductAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> ProductExistsAsync(Guid id)
    {
        return await _productRepository.ExistsAsync(id);
    }

    public async Task<bool> StoreExistsByCodeAsync(int storeCode)
    {
        var store = await _storeRepository.GetByCodeAsync(storeCode);
        return store != null;
    }
}
