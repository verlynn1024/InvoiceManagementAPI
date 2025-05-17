using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Domain.Entities;
using InvoiceManagementAPI.Domain.Enums;
using InvoiceManagementAPI.Domain.Events;
// Use alias to resolve ambiguity
using AppNotFoundException = InvoiceManagementAPI.Application.Common.Exceptions.NotFoundException;

namespace InvoiceManagementAPI.Application.Invoices.Commands.UpdateInvoiceStatus;

public record UpdateInvoiceStatusCommand : IRequest
{
    public int Id { get; init; }
    public InvoiceStatus Status { get; init; }
    public decimal? PaymentAmount { get; init; }
}

public class UpdateInvoiceStatusCommandHandler : IRequestHandler<UpdateInvoiceStatusCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateInvoiceStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Invoices
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            // Use the aliased namespace to avoid ambiguity
            throw new AppNotFoundException(nameof(Invoice), request.Id.ToString());
        }

        entity.Status = request.Status;

        // If this is a payment, update the payment amount
        if (request.Status == InvoiceStatus.Paid && request.PaymentAmount.HasValue)
        {
            entity.PaidAmount += request.PaymentAmount.Value;

            // If fully paid, add domain event
            if (entity.PaidAmount >= entity.TotalAmount)
            {
                entity.Status = InvoiceStatus.Paid;
                entity.AddDomainEvent(new InvoicePaidEvent(entity));
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
