using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PaymentMethodConfiguration : BaseEntityConfiguration<PaymentMethod>
    {
        public override void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            base.Configure(builder);

            builder.ToTable("PaymentMethods");

            builder.Property(pm => pm.Name)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(100);

            builder.HasMany(pm => pm.Payments)
                   .WithOne(p => p.PaymentMethod)
                   .HasForeignKey(p => p.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(pm => pm.PayInvoices)
                   .WithOne(pi => pi.PaymentMethod)
                   .HasForeignKey(pi => pi.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
