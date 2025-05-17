namespace InvoiceManagementAPI.Application.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Description)
            .MaximumLength(500);

        RuleFor(v => v.Price)
            .GreaterThanOrEqualTo(0);

        RuleFor(v => v.Sku)
            .MaximumLength(50);
    }
}
