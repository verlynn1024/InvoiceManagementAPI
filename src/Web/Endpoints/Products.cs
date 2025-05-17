using InvoiceManagementAPI.Application.Common.Models;
using InvoiceManagementAPI.Application.Products.Commands.CreateProduct;
using InvoiceManagementAPI.Application.Products.Queries.GetProductsWithPagination;
using Microsoft.AspNetCore.Http.HttpResults;

namespace InvoiceManagementAPI.Web.Endpoints;

public class Products : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetProductsWithPagination)
            .MapPost(CreateProduct);
    }

    public async Task<Ok<PaginatedList<ProductDto>>> GetProductsWithPagination(ISender sender, [AsParameters] GetProductsWithPaginationQuery query)
    {
        return TypedResults.Ok(await sender.Send(query));
    }

    public async Task<Created<int>> CreateProduct(ISender sender, CreateProductCommand command)
    {
        return TypedResults.Created($"/api/{nameof(Products)}/{await sender.Send(command)}", await sender.Send(command));
    }
}
