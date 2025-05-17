using InvoiceManagementAPI.Domain.Entities;

namespace InvoiceManagementAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceItem> InvoiceItems { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Product> Products { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
