using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class InvoiceItemConfiguration : BaseEntityConfiguration<InvoiceItem>
    {
        public override void Configure(EntityTypeBuilder<InvoiceItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("InvoiceItems");

            builder.Property(ii => ii.Quantity)
                   .IsRequired();

            builder.Property(ii => ii.UnitPrice)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(ii => ii.LineTotal)
                   .HasColumnType("decimal(18,2)")
                   .HasComputedColumnSql("[Quantity] * [UnitPrice]");

            builder.HasOne(ii => ii.Invoice)
                   .WithMany(i => i.InvoiceItems)
                   .HasForeignKey(ii => ii.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ii => ii.Product)
                   .WithMany(p => p.InvoiceItems)
                   .HasForeignKey(ii => ii.ProductId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
