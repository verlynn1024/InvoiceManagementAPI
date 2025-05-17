using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Application.Common.Mappings;
using InvoiceManagementAPI.Application.Common.Models;

namespace InvoiceManagementAPI.Application.Customers.Queries.GetCustomersWithPagination;

public record GetCustomersWithPaginationQuery : IRequest<PaginatedList<CustomerDto>>
{
    public string? SearchString { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetCustomersWithPaginationQueryHandler : IRequestHandler<GetCustomersWithPaginationQuery, PaginatedList<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCustomersWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Customers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(c =>
                c.Name.Contains(request.SearchString) ||
                c.Email.Contains(request.SearchString));
        }

        return await query
            .OrderBy(c => c.Name)
            .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
