using InvoiceManagementAPI.Domain.Entities;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;

public class InvoiceItemDto
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal Total { get; set; }
    public string? ProductName { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<InvoiceItem, InvoiceItemDto>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(s => s.Product != null ? s.Product.Name : null));
        }
    }
}
