using Microsoft.AspNetCore.Mvc;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;

    public CompaniesController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    /// <summary>
    /// Get all companies
    /// </summary>
    /// <returns>List of companies</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies()
    {
        var companies = await _companyService.GetAllCompaniesAsync();
        return Ok(companies);
    }

    /// <summary>
    /// Get active companies only
    /// </summary>
    /// <returns>List of active companies</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<CompanyDto>>> GetActiveCompanies()
    {
        var companies = await _companyService.GetActiveCompaniesAsync();
        return Ok(companies);
    }

    /// <summary>
    /// Get company by code
    /// </summary>
    /// <param name="code">Company Code</param>
    /// <returns>Company details</returns>
    [HttpGet("{code}")]
    public async Task<ActionResult<CompanyDto>> GetCompany(int code)
    {
        var company = await _companyService.GetCompanyByCodeAsync(code);
        if (company == null)
        {
            return NotFound($"Company with code {code} not found.");
        }

        return Ok(company);
    }

    /// <summary>
    /// Get company with its stores
    /// </summary>
    /// <param name="code">Company Code</param>
    /// <returns>Company details with stores</returns>
    [HttpGet("{code}/with-stores")]
    public async Task<ActionResult<CompanyDto>> GetCompanyWithStores(int code)
    {
        var company = await _companyService.GetCompanyWithStoresByCodeAsync(code);
        if (company == null)
        {
            return NotFound($"Company with code {code} not found.");
        }

        return Ok(company);
    }

    /// <summary>
    /// Create a new company
    /// </summary>
    /// <param name="createCompanyDto">Company creation data</param>
    /// <returns>Created company</returns>
    [HttpPost]
    public async Task<ActionResult<CompanyDto>> CreateCompany(CreateCompanyDto createCompanyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdCompany = await _companyService.CreateCompanyAsync(createCompanyDto);
            return CreatedAtAction(nameof(GetCompany), new { code = createdCompany.Code }, createdCompany);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing company completely (replaces all fields)
    /// </summary>
    /// <param name="code">Company Code</param>
    /// <param name="updateCompanyDto">Company update data</param>
    /// <returns>Updated company</returns>
    [HttpPut("{code}")]
    public async Task<ActionResult<CompanyDto>> UpdateCompany(int code, UpdateCompanyDto updateCompanyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _companyService.CompanyExistsByCodeAsync(code))
        {
            return NotFound($"Company with code {code} not found.");
        }

        try
        {
            var updatedCompany = await _companyService.UpdateCompanyByCodeAsync(code, updateCompanyDto);
            return Ok(updatedCompany);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Partially update an existing company (updates only provided fields)
    /// </summary>
    /// <param name="code">Company Code</param>
    /// <param name="patchCompanyDto">Company partial update data</param>
    /// <returns>Updated company</returns>
    [HttpPatch("{code}")]
    public async Task<ActionResult<CompanyDto>> PatchCompany(int code, PatchCompanyDto patchCompanyDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _companyService.CompanyExistsByCodeAsync(code))
        {
            return NotFound($"Company with code {code} not found.");
        }

        // Validate that at least one field is provided for patch
        if (patchCompanyDto.Name == null && patchCompanyDto.Code == null && patchCompanyDto.IsActive == null)
        {
            return BadRequest("At least one field must be provided for partial update.");
        }

        try
        {
            var updatedCompany = await _companyService.PatchCompanyByCodeAsync(code, patchCompanyDto);
            return Ok(updatedCompany);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a company
    /// </summary>
    /// <param name="code">Company Code</param>
    /// <returns>No content</returns>
    [HttpDelete("{code}")]
    public async Task<IActionResult> DeleteCompany(int code)
    {
        if (!await _companyService.CompanyExistsByCodeAsync(code))
        {
            return NotFound($"Company with code {code} not found.");
        }

        try
        {
            await _companyService.DeleteCompanyByCodeAsync(code);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
