
namespace StoreManagement.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int Code { get; set; }
    public virtual IList<Store> Stores { get; set; } = new List<Store>();
}