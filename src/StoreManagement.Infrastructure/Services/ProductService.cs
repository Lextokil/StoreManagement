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

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? _mapper.Map<ProductDto>(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByStoreAsync(Guid storeId)
    {
        var products = await _productRepository.GetByStoreIdAsync(storeId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetProductsByCompanyAsync(Guid companyId)
    {
        var products = await _productRepository.GetByCompanyIdAsync(companyId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(Guid companyId, int threshold = 10)
    {
        var products = await _productRepository.GetLowStockProductsAsync(companyId, threshold);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<string> GetProductsAsJsonAsync(Guid companyId)
    {
        return await _productRepository.GetProductsAsJsonAsync(companyId);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
    {
        // Validate store exists
        var store = await _storeRepository.GetByIdAsync(createProductDto.StoreId);
        if (store == null)
        {
            throw new ArgumentException($"Store with ID {createProductDto.StoreId} not found.");
        }

        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            StoreId = createProductDto.StoreId,
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

        if (product.StoreId != updateProductDto.StoreId)
        {
            var store = await _storeRepository.GetByIdAsync(updateProductDto.StoreId);
            if (store == null)
            {
                throw new ArgumentException($"Store with ID {updateProductDto.StoreId} not found.");
            }
        }

        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Price = updateProductDto.Price;
        product.StockQuantity = updateProductDto.StockQuantity;
        product.StoreId = updateProductDto.StoreId;

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

    public async Task<bool> StoreExistsAsync(Guid storeId)
    {
        return await _storeRepository.ExistsAsync(storeId);
    }

    public async Task<bool> CompanyExistsAsync(Guid companyId)
    {
        var stores = await _storeRepository.FindAsync(s => s.CompanyId == companyId);
        return stores.Any();
    }

    public async Task<int> GetProductCountAsync()
    {
        return await _productRepository.CountAsync();
    }

    public async Task<int> GetProductCountByStoreAsync(Guid storeId)
    {
        return await _productRepository.CountAsync(p => p.StoreId == storeId);
    }
}
