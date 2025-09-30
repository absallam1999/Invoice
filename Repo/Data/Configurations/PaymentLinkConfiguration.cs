﻿using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PaymentLinkConfiguration : BaseEntityConfiguration<PaymentLink>
    {
        public override void Configure(EntityTypeBuilder<PaymentLink> builder)
        {
            base.Configure(builder);

            builder.ToTable("PaymentLinks");

            builder.Property(pl => pl.Value)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(pl => pl.PaymentsNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(pl => pl.Description)
                   .HasMaxLength(1000);

            builder.HasOne(pl => pl.Invoice)
                   .WithOne(inv => inv.PaymentLink)
                   .HasForeignKey<PaymentLink>(pl => pl.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}