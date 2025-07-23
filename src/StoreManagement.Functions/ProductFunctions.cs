using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using StoreManagement.Domain.DTOs;
using StoreManagement.Domain.Interfaces.Services;

namespace StoreManagement.Functions;

public class ProductFunctions
{
    private readonly ILogger<ProductFunctions> _logger;
    private readonly IProductService _productService;

    public ProductFunctions(ILogger<ProductFunctions> logger, IProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [Function("GetProductsByStoreCode")]
    public async Task<HttpResponseData> GetProductsByStoreCode(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/store/{storeCode}")]
        HttpRequestData req,
        int storeCode)
    {
        _logger.LogInformation("Getting products for store code {StoreCode}", storeCode);

        try
        {
            if (!await _productService.StoreExistsByCodeAsync(storeCode))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Store with code {storeCode} not found");
                return notFoundResponse;
            }

            var products = await _productService.GetProductsByStoreCodeAsync(storeCode);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(products);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting products for store code {StoreCode}", storeCode);
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }

    [Function("GetProduct")]
    public async Task<HttpResponseData> GetProduct(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{id}")]
        HttpRequestData req,
        Guid id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);

        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Product with ID {id} not found");
                return notFoundResponse;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(product);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product {ProductId}", id);
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }

    [Function("CreateProduct")]
    public async Task<HttpResponseData> CreateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")]
        HttpRequestData req)
    {
        _logger.LogInformation("Creating new product");

        try
        {
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createProductDto = JsonSerializer.Deserialize<CreateProductDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (createProductDto == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid product data");
                return badRequestResponse;
            }

            if (!await _productService.StoreExistsByCodeAsync(createProductDto.StoreCode))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync($"Store with code {createProductDto.StoreCode} not found");
                return badRequestResponse;
            }

            var createdProduct = await _productService.CreateProductAsync(createProductDto);
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdProduct);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }

    [Function("UpdateProduct")]
    public async Task<HttpResponseData> UpdateProduct(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")]
        HttpRequestData req,
        Guid id)
    {
        _logger.LogInformation("Updating product {ProductId}", id);

        try
        {
            if (!await _productService.ProductExistsAsync(id))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Product with ID {id} not found");
                return notFoundResponse;
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateProductDto = JsonSerializer.Deserialize<UpdateProductDto>(requestBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (updateProductDto == null)
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync("Invalid product data");
                return badRequestResponse;
            }

            if (!await _productService.StoreExistsByCodeAsync(updateProductDto.StoreCode))
            {
                var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badRequestResponse.WriteStringAsync($"Store with code {updateProductDto.StoreCode} not found");
                return badRequestResponse;
            }

            var updatedProduct = await _productService.UpdateProductAsync(id, updateProductDto);
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedProduct);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", id);
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }

    [Function("DeleteProduct")]
    public async Task<HttpResponseData> DeleteProduct(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")]
        HttpRequestData req,
        Guid id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);

        try
        {
            if (!await _productService.ProductExistsAsync(id))
            {
                var notFoundResponse = req.CreateResponse(HttpStatusCode.NotFound);
                await notFoundResponse.WriteStringAsync($"Product with ID {id} not found");
                return notFoundResponse;
            }

            await _productService.DeleteProductAsync(id);
            var response = req.CreateResponse(HttpStatusCode.NoContent);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting product {ProductId}", id);
            var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
            await errorResponse.WriteStringAsync("Internal server error");
            return errorResponse;
        }
    }
}
