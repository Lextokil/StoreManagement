
namespace StoreManagement.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public virtual IList<Store> Stores { get; set; } = new List<Store>();
}