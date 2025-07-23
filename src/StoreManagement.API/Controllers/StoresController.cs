using Microsoft.AspNetCore.Mvc;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StoresController : ControllerBase
{
    private readonly IStoreService _storeService;

    public StoresController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    /// <summary>
    /// Get all stores
    /// </summary>
    /// <returns>List of stores</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StoreDto>>> GetStores()
    {
        var stores = await _storeService.GetAllStoresAsync();
        return Ok(stores);
    }

    /// <summary>
    /// Get stores by company code
    /// </summary>
    /// <param name="companyCode">Company Code</param>
    /// <returns>List of stores for the company</returns>
    [HttpGet("company/{companyCode}")]
    public async Task<ActionResult<IEnumerable<StoreDto>>> GetStoresByCompanyCode(int companyCode)
    {
        if (!await _storeService.CompanyExistsByCodeAsync(companyCode))
        {
            return NotFound($"Company with code {companyCode} not found.");
        }

        try
        {
            var stores = await _storeService.GetStoresByCompanyCodeAsync(companyCode);
            return Ok(stores);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get store by ID
    /// </summary>
    /// <param name="id">Store ID</param>
    /// <returns>Store details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<StoreDto>> GetStore(Guid id)
    {
        var store = await _storeService.GetStoreByIdAsync(id);
        if (store == null)
        {
            return NotFound();
        }

        return Ok(store);
    }

    /// <summary>
    /// Create a new store
    /// </summary>
    /// <param name="createStoreDto">Store creation data</param>
    /// <returns>Created store</returns>
    [HttpPost]
    public async Task<ActionResult<StoreDto>> CreateStore(CreateStoreDto createStoreDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _storeService.CompanyExistsByCodeAsync(createStoreDto.CompanyCode))
        {
            return BadRequest($"Company with code {createStoreDto.CompanyCode} not found.");
        }

        try
        {
            var createdStore = await _storeService.CreateStoreAsync(createStoreDto);
            return CreatedAtAction(nameof(GetStore), new { id = createdStore.Id }, createdStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing store
    /// </summary>
    /// <param name="id">Store ID</param>
    /// <param name="updateStoreDto">Store update data</param>
    /// <returns>Updated store</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<StoreDto>> UpdateStore(Guid id, UpdateStoreDto updateStoreDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _storeService.StoreExistsAsync(id))
        {
            return NotFound($"Store with ID {id} not found.");
        }

        if (!await _storeService.CompanyExistsByCodeAsync(updateStoreDto.CompanyCode))
        {
            return BadRequest($"Company with code {updateStoreDto.CompanyCode} not found.");
        }

        try
        {
            var updatedStore = await _storeService.UpdateStoreAsync(id, updateStoreDto);
            return Ok(updatedStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Partially update an existing store (updates only provided fields)
    /// </summary>
    /// <param name="id">Store ID</param>
    /// <param name="patchStoreDto">Store partial update data</param>
    /// <returns>Updated store</returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult<StoreDto>> PatchStore(Guid id, PatchStoreDto patchStoreDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _storeService.StoreExistsAsync(id))
        {
            return NotFound($"Store with ID {id} not found.");
        }

        // Validate that at least one field is provided for patch
        if (patchStoreDto.Name == null && patchStoreDto.Code == null && 
            patchStoreDto.Address == null && patchStoreDto.IsActive == null && 
            patchStoreDto.CompanyCode == null)
        {
            return BadRequest("At least one field must be provided for partial update.");
        }

        // Validate company code if provided
        if (patchStoreDto.CompanyCode.HasValue && 
            !await _storeService.CompanyExistsByCodeAsync(patchStoreDto.CompanyCode.Value))
        {
            return BadRequest($"Company with code {patchStoreDto.CompanyCode.Value} not found.");
        }

        try
        {
            var updatedStore = await _storeService.PatchStoreAsync(id, patchStoreDto);
            return Ok(updatedStore);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a store
    /// </summary>
    /// <param name="id">Store ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStore(Guid id)
    {
        if (!await _storeService.StoreExistsAsync(id))
        {
            return NotFound($"Store with ID {id} not found.");
        }

        try
        {
            await _storeService.DeleteStoreAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
