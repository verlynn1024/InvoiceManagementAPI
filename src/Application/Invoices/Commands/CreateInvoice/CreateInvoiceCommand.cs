using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Domain.Entities;
using InvoiceManagementAPI.Domain.Enums;
using InvoiceManagementAPI.Domain.Events;

namespace InvoiceManagementAPI.Application.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<int>
{
    public int CustomerId { get; init; }
    public DateTime? IssueDate { get; init; }
    public DateTime? DueDate { get; init; }
    public string Notes { get; init; } = string.Empty;
    public List<CreateInvoiceItemDto> Items { get; init; } = new();
}

public class CreateInvoiceItemDto
{
    public string Description { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal Quantity { get; init; }
    public int? ProductId { get; init; }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateInvoiceCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Generate a unique invoice number (this is a simple example - you might want a more sophisticated approach)
        var lastInvoice = await _context.Invoices
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync(cancellationToken);

        int invoiceNumber = 1000;
        if (lastInvoice != null)
        {
            if (int.TryParse(lastInvoice.InvoiceNumber.Replace("INV-", ""), out int lastNumber))
            {
                invoiceNumber = lastNumber + 1;
            }
        }

        // Create the invoice
        var entity = new Invoice
        {
            InvoiceNumber = $"INV-{invoiceNumber}",
            CustomerId = request.CustomerId,
            IssueDate = request.IssueDate ?? DateTime.Now,
            DueDate = request.DueDate ?? DateTime.Now.AddDays(30),
            Status = InvoiceStatus.Draft,
            Notes = request.Notes,
            TotalAmount = 0, // Will be calculated from items
            PaidAmount = 0
        };

        // Add invoice items
        foreach (var item in request.Items)
        {
            var invoiceItem = new InvoiceItem
            {
                Description = item.Description,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                Total = item.UnitPrice * item.Quantity,
                ProductId = item.ProductId
            };

            entity.Items.Add(invoiceItem);
            entity.TotalAmount += invoiceItem.Total;
        }

        entity.AddDomainEvent(new InvoiceCreatedEvent(entity));

        _context.Invoices.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
