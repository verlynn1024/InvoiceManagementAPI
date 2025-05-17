using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Application.Common.Mappings;
using InvoiceManagementAPI.Application.Common.Models;

namespace InvoiceManagementAPI.Application.Products.Queries.GetProductsWithPagination;

public record GetProductsWithPaginationQuery : IRequest<PaginatedList<ProductDto>>
{
    public string? SearchString { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetProductsWithPaginationQueryHandler : IRequestHandler<GetProductsWithPaginationQuery, PaginatedList<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetProductsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductDto>> Handle(GetProductsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(p =>
                p.Name.Contains(request.SearchString) ||
                p.Description.Contains(request.SearchString) ||
                p.Sku.Contains(request.SearchString));
        }

        return await query
            .OrderBy(p => p.Name)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
