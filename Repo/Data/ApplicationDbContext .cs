using System.Linq.Expressions;
﻿using invoice.Core.Entities;
using invoice.Core.Enums;
using invoice.Migrations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace invoice.Repo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }
        //public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<PayInvoice> PayInvoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentLink> PaymentLinks { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Tax> Taxes { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Order> Orders { get; set; }




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var deletedProp = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var compare = Expression.Equal(deletedProp, Expression.Constant(false));
                    var lambda = Expression.Lambda(compare, parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

            builder.Entity<Language>().HasData(
                new Language { Id = "ar", Name = LanguageName.Arabic },
                new Language { Id = "en", Name = LanguageName.English }
            );

            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod { Id = "ca", Name = PaymentType.Cash },
                new PaymentMethod { Id = "cc", Name = PaymentType.CreditCard },
                new PaymentMethod { Id = "dc", Name = PaymentType.DebitCard },
                new PaymentMethod { Id = "bt", Name = PaymentType.BankTransfer },
                new PaymentMethod { Id = "pp", Name = PaymentType.PayPal },
                new PaymentMethod { Id = "st", Name = PaymentType.Stripe },
                new PaymentMethod { Id = "ap", Name = PaymentType.ApplePay },
                new PaymentMethod { Id = "gp", Name = PaymentType.GooglePay },
                new PaymentMethod { Id = "ma", Name = PaymentType.Mada },
                new PaymentMethod { Id = "sp", Name = PaymentType.STCPay },
                new PaymentMethod { Id = "sa", Name = PaymentType.Sadad },
                new PaymentMethod { Id = "dl", Name = PaymentType.Delivery }
            );

            builder.Entity<Store>(store =>
            {
                store.OwnsOne(s => s.PaymentOptions, po =>
                {
                    po.Property(p => p.Cash).HasDefaultValue(true);
                    po.Property(p => p.BankTransfer).HasDefaultValue(false);
                    po.Property(p => p.PayPal).HasDefaultValue(false);
                    po.Property(p => p.Tax).HasDefaultValue(false);
                });

                store.OwnsOne(s => s.ContactInformations, ci =>
                {
                    ci.Property(c => c.Phone)
                      .HasMaxLength(20)
                      .IsRequired(false);

                    ci.Property(c => c.Email)
                      .HasMaxLength(100)
                      .IsRequired(false);

                    ci.Property(c => c.Location)
                      .HasMaxLength(200)
                      .IsRequired(false);

                    ci.Property(c => c.Facebook)
                      .HasMaxLength(150)
                      .IsRequired(false);

                    ci.Property(c => c.WhatsApp)
                      .HasMaxLength(20)
                      .IsRequired(false);

                    ci.Property(c => c.Instagram)
                      .HasMaxLength(150)
                      .IsRequired(false);
                });
            });
        }



    }


}
