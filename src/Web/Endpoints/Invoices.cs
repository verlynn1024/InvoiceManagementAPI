using InvoiceManagementAPI.Application.Common.Models;
using InvoiceManagementAPI.Application.Invoices.Commands.CreateInvoice;
using InvoiceManagementAPI.Application.Invoices.Commands.UpdateInvoiceStatus;
using InvoiceManagementAPI.Application.Invoices.Queries.GetInvoiceDetail;
using InvoiceManagementAPI.Application.Invoices.Queries.GetInvoicesWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InvoiceManagementAPI.Web.Endpoints;

public class Invoices : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetInvoicesWithPagination)
            .MapGet(GetInvoiceDetail, "{id}")
            .MapPost(CreateInvoice)
            .MapPut(UpdateInvoiceStatus, "status/{id}");
    }

    public async Task<Ok<PaginatedList<InvoiceDto>>> GetInvoicesWithPagination(ISender sender, [AsParameters] GetInvoicesWithPaginationQuery query)
    {
        return TypedResults.Ok(await sender.Send(query));
    }

    public async Task<Results<Ok<InvoiceDetailDto>, NotFound>> GetInvoiceDetail(ISender sender, int id)
    {
        try
        {
            return TypedResults.Ok(await sender.Send(new GetInvoiceDetailQuery(id)));
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }

    public async Task<Created<int>> CreateInvoice(ISender sender, CreateInvoiceCommand command)
    {
        return TypedResults.Created($"/api/{nameof(Invoices)}/{await sender.Send(command)}", await sender.Send(command));
    }

    public async Task<Results<NoContent, NotFound>> UpdateInvoiceStatus(ISender sender, int id, UpdateInvoiceStatusCommand command)
    {
        if (id != command.Id) return TypedResults.NotFound();

        try
        {
            await sender.Send(command);
            return TypedResults.NoContent();
        }
        catch (NotFoundException)
        {
            return TypedResults.NotFound();
        }
    }
}
