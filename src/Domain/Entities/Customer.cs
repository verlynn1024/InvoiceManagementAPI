namespace InvoiceManagementAPI.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;
    public IList<Invoice> Invoices { get; private set; } = new List<Invoice>();
}
