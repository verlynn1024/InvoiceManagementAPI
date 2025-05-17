using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Domain.Entities;

namespace InvoiceManagementAPI.Application.Products.Commands.CreateProduct;

public record CreateInvoiceCommand : IRequest<int>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Sku { get; init; } = string.Empty;
}

public class CreateProductCommandHandler : IRequestHandler<CreateInvoiceCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreateProductCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var entity = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Sku = request.Sku
        };

        _context.Products.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
