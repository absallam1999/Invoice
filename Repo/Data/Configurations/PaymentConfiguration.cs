using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PaymentConfiguration : BaseEntityConfiguration<Payment>
    {
        public override void Configure(EntityTypeBuilder<Payment> builder)
        {
            base.Configure(builder);

            builder.ToTable("Payments");

            builder.Property(p => p.Name)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(p => p.Cost)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Link)
                   .HasMaxLength(1000);

            builder.Property(p => p.Currency)
                   .HasMaxLength(3)
                   .IsRequired();

            builder.Property(p => p.GatewaySessionId)
                   .HasMaxLength(1000);

            builder.Property(p => p.Status)
                   .IsRequired();

            builder.Property(p => p.Type)
                   .IsRequired();

            builder.Property(p => p.ExpiresAt)
                   .HasDefaultValueSql("DATEADD(DAY, 3, GETUTCDATE())");

            builder.HasIndex(p => p.GatewaySessionId)
                   .IsUnique();

            builder.HasOne(p => p.User)
                   .WithMany(u => u.Payments)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Invoice)
                   .WithMany(i => i.Payments)
                   .HasForeignKey(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.PaymentMethod)
                   .WithMany(pm => pm.Payments)
                   .HasForeignKey(p => p.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
