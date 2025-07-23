
namespace StoreManagement.Domain.Entities;

public class Store : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    
    public int Code { get; set; }

    public string? Address { get; set; }

    public Guid CompanyId { get; set; }

    public virtual Company Company { get; set; } = null!;
    public virtual IList<Product> Products { get; set; } = new List<Product>();
}