namespace StoreManagement.Domain.DTOs;

public record StoreDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int CompanyCode { get; set; }
}

public record CreateStoreDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public int CompanyCode { get; set; }
}

public record UpdateStoreDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public int CompanyCode { get; set; }
}

public record PatchStoreDto
{
    public string? Name { get; set; }
    public int? Code { get; set; }
    public string? Address { get; set; }
    public bool? IsActive { get; set; }
    public int? CompanyCode { get; set; }
}
