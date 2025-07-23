using AutoMapper;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Entities;
using StoreManagement.Domain.Interfaces;
using StoreManagement.Domain.Interfaces.Repositories;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.Infrastructure.Services;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CompanyService(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
    {
        var companies = await _companyRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<IEnumerable<CompanyDto>> GetActiveCompaniesAsync()
    {
        var companies = await _companyRepository.GetActiveCompaniesAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<CompanyDto?> GetCompanyByIdAsync(Guid id)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<CompanyDto?> GetCompanyByCodeAsync(int code)
    {
        var company = await _companyRepository.GetByCodeAsync(code);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<CompanyDto?> GetCompanyWithStoresAsync(Guid id)
    {
        var company = await _companyRepository.GetCompanyWithStoresAsync(id);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<CompanyDto?> GetCompanyWithStoresByCodeAsync(int code)
    {
        var company = await _companyRepository.GetCompanyWithStoresByCodeAsync(code);
        return company != null ? _mapper.Map<CompanyDto>(company) : null;
    }

    public async Task<CompanyDto> CreateCompanyAsync(CreateCompanyDto createCompanyDto)
    {
        var company = new Company
        {
            Name = createCompanyDto.Name,
            Code = createCompanyDto.Code,
            IsActive = createCompanyDto.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(company);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> UpdateCompanyAsync(Guid id, UpdateCompanyDto updateCompanyDto)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null)
            throw new ArgumentException($"Company with ID {id} not found.");

        company.Name = updateCompanyDto.Name;
        company.Code = updateCompanyDto.Code;
        company.IsActive = updateCompanyDto.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        _companyRepository.UpdateAsync(company);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> PatchCompanyAsync(Guid id, PatchCompanyDto patchCompanyDto)
    {
        var company = await _companyRepository.GetByIdAsync(id);
        if (company == null)
            throw new ArgumentException($"Company with ID {id} not found.");

        // Update only provided fields
        if (patchCompanyDto.Name != null)
            company.Name = patchCompanyDto.Name;

        if (patchCompanyDto.Code.HasValue)
            company.Code = patchCompanyDto.Code.Value;

        if (patchCompanyDto.IsActive.HasValue)
            company.IsActive = patchCompanyDto.IsActive.Value;

        company.UpdatedAt = DateTime.UtcNow;

        _companyRepository.UpdateAsync(company);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task DeleteCompanyAsync(Guid id)
    {
        await _companyRepository.DeleteAsync(id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<CompanyDto> UpdateCompanyByCodeAsync(int code, UpdateCompanyDto updateCompanyDto)
    {
        var company = await _companyRepository.GetByCodeAsync(code);
        if (company == null)
            throw new ArgumentException($"Company with code {code} not found.");

        company.Name = updateCompanyDto.Name;
        company.Code = updateCompanyDto.Code;
        company.IsActive = updateCompanyDto.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        _companyRepository.UpdateAsync(company);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task<CompanyDto> PatchCompanyByCodeAsync(int code, PatchCompanyDto patchCompanyDto)
    {
        var company = await _companyRepository.GetByCodeAsync(code);
        if (company == null)
            throw new ArgumentException($"Company with code {code} not found.");

        // Update only provided fields
        if (patchCompanyDto.Name != null)
            company.Name = patchCompanyDto.Name;

        if (patchCompanyDto.Code.HasValue)
            company.Code = patchCompanyDto.Code.Value;

        if (patchCompanyDto.IsActive.HasValue)
            company.IsActive = patchCompanyDto.IsActive.Value;

        company.UpdatedAt = DateTime.UtcNow;

        _companyRepository.UpdateAsync(company);
        await _unitOfWork.CommitAsync();

        return _mapper.Map<CompanyDto>(company);
    }

    public async Task DeleteCompanyByCodeAsync(int code)
    {
        var company = await _companyRepository.GetByCodeAsync(code);
        if (company == null)
            throw new ArgumentException($"Company with code {code} not found.");

        await _companyRepository.DeleteAsync(company.Id);
        await _unitOfWork.CommitAsync();
    }

    public async Task<bool> CompanyExistsAsync(Guid id)
    {
        return await _companyRepository.ExistsAsync(id);
    }

    public async Task<bool> CompanyExistsByCodeAsync(int code)
    {
        var company = await _companyRepository.GetByCodeAsync(code);
        return company != null;
    }
}
