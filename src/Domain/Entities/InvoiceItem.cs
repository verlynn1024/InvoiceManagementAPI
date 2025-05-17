namespace InvoiceManagementAPI.Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal Total { get; set; }
    public int InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;

    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}
