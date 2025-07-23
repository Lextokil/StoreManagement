namespace StoreManagement.Domain.DTOs;

public record ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid StoreId { get; set; }
    public string StoreName { get; set; } = string.Empty;
    public int StoreCode { get; set; }
}

public record CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int StoreCode { get; set; }
}

public record UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
    public int StoreCode { get; set; }
}

public record PatchProductDto
{
    public string? Name { get; set; }
    public int? Code { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public bool? IsActive { get; set; }
    public int? StoreCode { get; set; }
}
