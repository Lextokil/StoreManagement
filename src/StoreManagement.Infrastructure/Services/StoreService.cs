using AutoMapper;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.Infrastructure.Services;

public class StoreService : IStoreService
{
    private readonly IStoreRepository _storeRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StoreService(
        IStoreRepository storeRepository,
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _storeRepository = storeRepository;
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<StoreDto>> GetAllStoresAsync()
    {
        var stores = await _storeRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StoreDto>>(stores);
    }

    public async Task<IEnumerable<StoreDto>> GetStoresByCompanyAsync(Guid companyId)
    {
        var stores = await _storeRepository.GetByCompanyIdAsync(companyId);
        return _mapper.Map<IEnumerable<StoreDto>>(stores);
    }

    public async Task<StoreDto?> GetStoreByIdAsync(Guid id)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        return store != null ? _mapper.Map<StoreDto>(store) : null;
    }

    public async Task<StoreDto> CreateStoreAsync(CreateStoreDto createStoreDto)
    {
        // Validate company exists
        var company = await _companyRepository.GetByIdAsync(createStoreDto.CompanyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {createStoreDto.CompanyId} not found.");
        }

        var store = new Store
        {
            Name = createStoreDto.Name,
            Address = createStoreDto.Address,
            IsActive = createStoreDto.IsActive,
            CompanyId = createStoreDto.CompanyId,
            CreatedAt = DateTime.UtcNow
        };

        await _storeRepository.AddAsync(store);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<StoreDto>(store);
    }

    public async Task<StoreDto> UpdateStoreAsync(Guid id, UpdateStoreDto updateStoreDto)
    {
        var store = await _storeRepository.GetByIdAsync(id);
        if (store == null)
            throw new ArgumentException($"Store with ID {id} not found.");

        // Validate company exists if it's being changed
        if (store.CompanyId != updateStoreDto.CompanyId)
        {
            var company = await _companyRepository.GetByIdAsync(updateStoreDto.CompanyId);
            if (company == null)
            {
                throw new ArgumentException($"Company with ID {updateStoreDto.CompanyId} not found.");
            }
        }

        store.Name = updateStoreDto.Name;
        store.Address = updateStoreDto.Address;
        store.IsActive = updateStoreDto.IsActive;
        store.CompanyId = updateStoreDto.CompanyId;

        _storeRepository.UpdateAsync(store);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<StoreDto>(store);
    }

    public async Task DeleteStoreAsync(Guid id)
    {
        await _storeRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> StoreExistsAsync(Guid id)
    {
        return await _storeRepository.ExistsAsync(id);
    }

    public async Task<bool> CompanyExistsAsync(Guid companyId)
    {
        return await _companyRepository.ExistsAsync(companyId);
    }
}
