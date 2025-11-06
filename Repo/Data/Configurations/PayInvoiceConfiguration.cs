using invoice.Core.Entities;
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

            builder.Property(p => p.PaidAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETUTCDATE()");

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