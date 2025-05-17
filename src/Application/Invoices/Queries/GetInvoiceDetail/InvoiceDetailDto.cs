using InvoiceManagementAPI.Domain.Entities;
using InvoiceManagementAPI.Domain.Enums;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;

public class GetProductsWithPaginationQuery
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Notes { get; set; } = string.Empty;

    public GetCustomersWithPaginationQuery Customer { get; set; } = null!;

    public IList<GetInvoicesWithPaginationQuery> Items { get; set; } = new List<GetInvoicesWithPaginationQuery>();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Invoice, GetProductsWithPaginationQuery>();
        }
    }
}
