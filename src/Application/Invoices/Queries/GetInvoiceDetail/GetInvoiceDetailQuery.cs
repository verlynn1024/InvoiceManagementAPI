
using InvoiceManagementAPI.Application.Common.Interfaces;
using AppNotFoundException = InvoiceManagementAPI.Application.Common.Exceptions.NotFoundException;

namespace InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;

public record GetInvoiceDetailQuery(int Id) : IRequest<InvoiceDetailDto>;

public class GetInvoiceDetailQueryHandler : IRequestHandler<GetInvoiceDetailQuery, InvoiceDetailDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetInvoiceDetailQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InvoiceDetailDto> Handle(GetInvoiceDetailQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

        if (invoice == null)
        {
            throw new AppNotFoundException(nameof(Invoice), request.Id.ToString());
        }

        var invoiceDto = _mapper.Map<InvoiceDetailDto>(invoice);
        invoiceDto.Customer = _mapper.Map<CustomerSummaryDto>(invoice.Customer);
        invoiceDto.Items = _mapper.Map<List<InvoiceItemDto>>(invoice.Items);

        return invoiceDto;
    }
}
