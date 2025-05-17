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
            _context.Customers.AddRange(
                new Customer
                {
                    Name = "Acme Corporation",
                    Email = "accounting@acme.com",
                    Phone = "555-123-4567",
                    Address = "123 Main St, Somewhere, US 12345",
                    TaxId = "AC-987654321",
                },
                new Customer
                {
                    Name = "Globex Industries",
                    Email = "finance@globex.com",
                    Phone = "555-987-6543",
                    Address = "456 Business Ave, Anywhere, US 54321",
                    TaxId = "GI-123456789",
                }
            );

            await _context.SaveChangesAsync();
        }

        if (!_context.Products.Any())
        {
            _context.Products.AddRange(
                new Product
                {
                    Name = "Web Development Service",
                    Description = "Professional web development services by the hour",
                    Price = 150.00m,
                    Sku = "WEB-DEV-001"
                },
                new Product
                {
                    Name = "Mobile App Development",
                    Description = "Professional mobile app development services by the hour",
                    Price = 175.00m,
                    Sku = "MOB-DEV-001"
                },
                new Product
                {
                    Name = "DevOps Consulting",
                    Description = "Expert DevOps consulting and implementation",
                    Price = 200.00m,
                    Sku = "DEVOPS-001"
                }
            );

            await _context.SaveChangesAsync();
        }

        if (!_context.Invoices.Any())
        {
            var customer = await _context.Customers.FirstAsync();
            var products = await _context.Products.ToListAsync();

            var invoice = new Invoice
            {
                InvoiceNumber = "INV-1001",
                CustomerId = customer.Id,
                IssueDate = DateTime.Now.AddDays(-15),
                DueDate = DateTime.Now.AddDays(15),
                Status = InvoiceStatus.Sent,
                Notes = "Sample invoice for demo purposes",
                Items = new List<InvoiceItem>
                {
                    new InvoiceItem
                    {
                        Description = products[0].Name,
                        UnitPrice = products[0].Price,
                        Quantity = 10,
                        Total = products[0].Price * 10,
                        ProductId = products[0].Id
                    },
                    new InvoiceItem
                    {
                        Description = "Custom Website Design",
                        UnitPrice = 2500.00m,
                        Quantity = 1,
                        Total = 2500.00m
                    }
                }
            };

            // Calculate total
            invoice.TotalAmount = invoice.Items.Sum(i => i.Total);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }
    }
}
