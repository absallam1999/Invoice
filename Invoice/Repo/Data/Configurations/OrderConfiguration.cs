using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class OrderConfiguration : BaseEntityConfiguration<Order>
    {
        //    public override void Configure(EntityTypeBuilder<Order> builder)
        //    {
        //        base.Configure(builder);

        //        builder.ToTable("Orders");

        //        builder.Property(o => o.Code)
        //               .IsRequired()
        //               .HasMaxLength(50);

        //        builder.HasIndex(o => o.Code)
        //               .IsUnique();

        //        builder.Property(o => o.OrderStatus)
        //               .IsRequired();

        //        builder.Property(o => o.TotalAmount)
        //               .HasColumnType("decimal(18,2)")
        //               .IsRequired();

        //        builder.HasOne(o => o.Store)
        //               .WithMany(s => s.Orders)
        //               .HasForeignKey(o => o.StoreId)
        //               .OnDelete(DeleteBehavior.Restrict);

        //        builder.HasOne(o => o.Invoice)
        //               .WithOne(i => i.Order)
        //               .HasForeignKey<Order>(o => o.InvoiceId)
        //               .OnDelete(DeleteBehavior.Restrict);

        //        builder.HasOne(o => o.Client)
        //               .WithMany(c => c.Orders)
        //               .HasForeignKey(o => o.ClientId)
        //               .OnDelete(DeleteBehavior.Restrict);

        //        builder.HasMany(o => o.OrderItems)
        //               .WithOne(oi => oi.Order)
        //               .HasForeignKey(oi => oi.OrderId)
        //               .OnDelete(DeleteBehavior.Cascade);
    }
}

