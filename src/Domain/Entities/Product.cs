namespace InvoiceManagementAPI.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Sku { get; set; } = string.Empty;
    public IList<InvoiceItem> InvoiceItems { get; private set; } = new List<InvoiceItem>();
}
