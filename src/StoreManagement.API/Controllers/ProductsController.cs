using Microsoft.AspNetCore.Mvc;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get products by store code
    /// </summary>
    /// <param name="storeCode">Store Code</param>
    /// <returns>List of products for the store</returns>
    [HttpGet("store/{storeCode}")]
    public async Task<ActionResult<string>> GetProductsByStoreCode(int storeCode)
    {
        if (!await _productService.StoreExistsByCodeAsync(storeCode))
        {
            return NotFound($"Store with code {storeCode} not found.");
        }

        try
        {
            var products = await _productService.GetProductsByStoreCodeAsync(storeCode);
            return Ok(products);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>Product details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <param name="createProductDto">Product creation data</param>
    /// <returns>Created product</returns>
    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _productService.StoreExistsByCodeAsync(createProductDto.StoreCode))
        {
            return BadRequest($"Store with code {createProductDto.StoreCode} not found.");
        }

        try
        {
            var createdProduct = await _productService.CreateProductAsync(createProductDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="updateProductDto">Product update data</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _productService.ProductExistsAsync(id))
        {
            return NotFound($"Product with ID {id} not found.");
        }

        if (!await _productService.StoreExistsByCodeAsync(updateProductDto.StoreCode))
        {
            return BadRequest($"Store with code {updateProductDto.StoreCode} not found.");
        }

        try
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Partially update an existing product (updates only provided fields)
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <param name="patchProductDto">Product partial update data</param>
    /// <returns>Updated product</returns>
    [HttpPatch("{id}")]
    public async Task<ActionResult<ProductDto>> PatchProduct(Guid id, PatchProductDto patchProductDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!await _productService.ProductExistsAsync(id))
        {
            return NotFound($"Product with ID {id} not found.");
        }

        // Validate that at least one field is provided for patch
        if (patchProductDto.Name == null && patchProductDto.Code == null && 
            patchProductDto.Description == null && patchProductDto.Price == null && 
            patchProductDto.IsActive == null && patchProductDto.StoreCode == null)
        {
            return BadRequest("At least one field must be provided for partial update.");
        }

        // Validate store code if provided
        if (patchProductDto.StoreCode.HasValue && 
            !await _productService.StoreExistsByCodeAsync(patchProductDto.StoreCode.Value))
        {
            return BadRequest($"Store with code {patchProductDto.StoreCode.Value} not found.");
        }

        try
        {
            var updatedProduct = await _productService.PatchProductAsync(id, patchProductDto);
            return Ok(updatedProduct);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    /// <param name="id">Product ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        if (!await _productService.ProductExistsAsync(id))
        {
            return NotFound($"Product with ID {id} not found.");
        }

        try
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
