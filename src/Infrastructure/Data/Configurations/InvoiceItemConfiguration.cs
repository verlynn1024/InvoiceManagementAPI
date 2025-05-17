using InvoiceManagementAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InvoiceManagementAPI.Infrastructure.Data.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.Property(i => i.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(i => i.Quantity)
            .HasPrecision(18, 2);

        builder.Property(i => i.Total)
            .HasPrecision(18, 2);

        builder.HasOne(i => i.Invoice)
            .WithMany(invoice => invoice.Items)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.InvoiceItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
