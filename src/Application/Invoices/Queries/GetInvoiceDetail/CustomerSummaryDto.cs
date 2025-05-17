using InvoiceManagementAPI.Domain.Entities;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;

public class CustomerSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Customer, CustomerSummaryDto>();
        }
    }
}
