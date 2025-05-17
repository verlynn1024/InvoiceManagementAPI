using InvoiceManagementAPI.Application.Common.Interfaces;
using InvoiceManagementAPI.Application.Common.Mappings;
using InvoiceManagementAPI.Application.Common.Models;
using InvoiceManagementAPI.Domain.Enums;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoicesWithPagination;

public record GetInvoicesWithPaginationQuery : IRequest<PaginatedList<InvoiceDto>>
{
    public string? SearchString { get; init; }
    public InvoiceStatus? Status { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetInvoicesWithPaginationQueryHandler : IRequestHandler<GetInvoicesWithPaginationQuery, PaginatedList<InvoiceDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInvoicesWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<InvoiceDto>> Handle(GetInvoicesWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Invoices
            .Include(i => i.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchString))
        {
            query = query.Where(i =>
                i.InvoiceNumber.Contains(request.SearchString) ||
                i.Customer.Name.Contains(request.SearchString));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(i => i.Status == request.Status.Value);
        }

        return await query
            .OrderByDescending(i => i.IssueDate)
            .ProjectTo<InvoiceDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
    }
}
