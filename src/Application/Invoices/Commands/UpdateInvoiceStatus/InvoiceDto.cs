using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InvoiceManagementAPI.Domain.Entities;
using InvoiceManagementAPI.Domain.Enums;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoicesWithPagination;

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.Customer.Name));
        }
    }
}
