namespace StoreManagement.Domain.DTOs;

public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCompanyDto
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public bool IsActive { get; set; }
}

public class PatchCompanyDto
{
    public string? Name { get; set; }
    public int? Code { get; set; }
    public bool? IsActive { get; set; }
}
