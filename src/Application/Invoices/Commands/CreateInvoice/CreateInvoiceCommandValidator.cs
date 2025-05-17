using InvoiceManagementAPI.Application.Common.Interfaces;

namespace InvoiceManagementAPI.Application.Invoices.Commands.CreateInvoice;

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateInvoiceCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(v => v.CustomerId)
            .NotEmpty()
            .MustAsync(CustomerExists)
                .WithMessage("Customer with specified ID does not exist.");

        RuleFor(v => v.Items)
            .NotEmpty()
            .WithMessage("At least one item is required.");

        RuleForEach(v => v.Items).ChildRules(item => {
            item.RuleFor(x => x.Description).NotEmpty().MaximumLength(200);
            item.RuleFor(x => x.UnitPrice).GreaterThan(0);
            item.RuleFor(x => x.Quantity).GreaterThan(0);
            item.RuleFor(x => x.ProductId)
                .MustAsync(ProductExists)
                .When(x => x.ProductId.HasValue)
                .WithMessage("Product with specified ID does not exist.");
        });
    }

    private async Task<bool> CustomerExists(int customerId, CancellationToken cancellationToken)
    {
        return await _context.Customers.AnyAsync(c => c.Id == customerId, cancellationToken);
    }

    private async Task<bool> ProductExists(int? productId, CancellationToken cancellationToken)
    {
        if (!productId.HasValue)
            return true;

        return await _context.Products.AnyAsync(p => p.Id == productId.Value, cancellationToken);
    }
}
