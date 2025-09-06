using invoice.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.ToTable("Orders");

            builder.Property(o => o.OrderStatus)
                   .IsRequired();

            builder.HasOne(o => o.Store)
                   .WithMany(s => s.Orders)
                   .HasForeignKey(o => o.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Invoice)
                   .WithMany(i => i.Orders)
                   .HasForeignKey(o => o.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Client)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                   .WithOne(oi => oi.Order)
                   .HasForeignKey(oi => oi.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
