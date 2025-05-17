namespace InvoiceManagementAPI.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string Notes { get; set; } = string.Empty;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public IList<InvoiceItem> Items { get; private set; } = new List<InvoiceItem>();
}
