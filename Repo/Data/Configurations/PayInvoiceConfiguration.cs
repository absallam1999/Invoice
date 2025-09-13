using invoice.Core.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PayInvoiceConfiguration : BaseEntityConfiguration<PayInvoice>
    {
        public override void Configure(EntityTypeBuilder<PayInvoice> builder)
        {
            base.Configure(builder);

            builder.ToTable("PayInvoices");

            builder.Property(p => p.Amount)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(p => p.RefundAmount)
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.PaidAt)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(p => p.Currency)
                   .IsRequired()
                   .HasMaxLength(10);

            builder.Property(p => p.PaymentSessionId)
                   .HasMaxLength(200);

            builder.Property(p => p.PaymentGatewayResponse)
                   .HasColumnType("nvarchar(max)");

            builder.HasOne(p => p.PaymentMethod)
                   .WithMany(pm => pm.PayInvoices)
                   .HasForeignKey(p => p.PaymentMethodId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Invoice)
                   .WithOne(i => i.PayInvoice)
                   .HasForeignKey<PayInvoice>(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
