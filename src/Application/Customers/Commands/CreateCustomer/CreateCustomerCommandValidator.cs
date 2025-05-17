using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvoiceManagementAPI.Application.Customers.Commands.CreateCustomer;

public class CreateProductCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(v => v.Name)
            .MaximumLength(100)
            .NotEmpty();

        RuleFor(v => v.Email)
            .MaximumLength(100)
            .EmailAddress()
            .When(v => !string.IsNullOrWhiteSpace(v.Email));

        RuleFor(v => v.Phone)
            .MaximumLength(20);

        RuleFor(v => v.Address)
            .MaximumLength(200);

        RuleFor(v => v.TaxId)
            .MaximumLength(30);
    }
}
