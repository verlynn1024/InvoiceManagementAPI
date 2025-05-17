using InvoiceManagementAPI.Domain.Constants;
using InvoiceManagementAPI.Domain.Entities;
using InvoiceManagementAPI.Domain.Enums;
using InvoiceManagementAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace InvoiceManagementAPI.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        // Default roles
        var administratorRole = new IdentityRole(Roles.Administrator);

        if (_roleManager.Roles.All(r => r.Name != administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        var administrator = new ApplicationUser { UserName = "administrator@localhost", Email = "administrator@localhost" };

        if (_userManager.Users.All(u => u.UserName != administrator.UserName))
        {
            await _userManager.CreateAsync(administrator, "Administrator1!");
            if (!string.IsNullOrWhiteSpace(administratorRole.Name))
            {
                await _userManager.AddToRolesAsync(administrator, new[] { administratorRole.Name });
            }
        }

        // Default data
        // Seed, if necessary
        if (!_context.Customers.Any())
        {
            // Add a sample customer
            var customer = new Customer
            {
                Name = "Sample Customer",
                Email = "sample@example.com",
                Phone = "555-1234",
                Address = "123 Main St",
                TaxId = "TAX12345"
            };

            _context.Customers.Add(customer);

            // Add a sample product
            var product = new Product
            {
                Name = "Sample Product",
                Description = "This is a sample product",
                Price = 99.99m,
                Sku = "PROD001"
            };

            _context.Products.Add(product);

            // Create a sample invoice
            var invoice = new Invoice
            {
                InvoiceNumber = "INV-1000",
                CustomerId = 1, // Will be assigned after save
                IssueDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(30),
                Status = InvoiceStatus.Draft,
                Notes = "Sample invoice",
                TotalAmount = 99.99m,
                PaidAmount = 0
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            // Now that we have IDs, add an invoice item
            var invoiceItem = new InvoiceItem
            {
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                Description = product.Name,
                UnitPrice = product.Price,
                Quantity = 1,
                Total = product.Price
            };

            _context.InvoiceItems.Add(invoiceItem);
            await _context.SaveChangesAsync();
        }
    }
}
