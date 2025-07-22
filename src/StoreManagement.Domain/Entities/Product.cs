
namespace StoreManagement.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Guid StoreId { get; set; }

    public virtual Store Store { get; set; } = null!;
}