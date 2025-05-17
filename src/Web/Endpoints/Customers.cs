using InvoiceManagementAPI.Application.Common.Models;
using InvoiceManagementAPI.Application.Customers.Commands.CreateCustomer;
using InvoiceManagementAPI.Application.Customers.Queries.GetCustomersWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InvoiceManagementAPI.Web.Endpoints;

public class Customers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetCustomersWithPagination)
            .MapPost(CreateCustomer);
    }

    public async Task<Ok<PaginatedList<CustomerDto>>> GetCustomersWithPagination(ISender sender, [AsParameters] GetCustomersWithPaginationQuery query)
    {
        return TypedResults.Ok(await sender.Send(query));
    }

    public async Task<Created<int>> CreateCustomer(ISender sender, CreateCustomerCommand command)
    {
        return TypedResults.Created($"/api/{nameof(Customers)}/{await sender.Send(command)}", await sender.Send(command));
    }
}
