using System.Linq.Expressions;
using invoice.Core.Entites;
using invoice.Core.Enums;
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
        public DbSet<ContactInfo> ContactInfos { get; set; }
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

            builder.Entity<Language>()
                .Property(l => l.Name)
                .HasConversion<string>();

            builder.Entity<Language>()
                .Property(l => l.Target)
                .HasConversion<string>();

            builder.Entity<PaymentMethod>()
                .Property(pm => pm.Name)
                .HasConversion<string>();

            builder.Entity<Language>().HasIndex(l => l.Id).IsUnique();
            builder.Entity<PaymentMethod>().HasIndex(pm => pm.Id).IsUnique();

            builder.Entity<Language>().HasData(
                new Language { Id = "ar_p", Name = LanguageName.Arabic, Target = LanguageTarget.Page },
                new Language { Id = "en_p", Name = LanguageName.English, Target = LanguageTarget.Page },
                new Language { Id = "ar_s", Name = LanguageName.Arabic, Target = LanguageTarget.Store },
                new Language { Id = "en_s", Name = LanguageName.English, Target = LanguageTarget.Store },
                new Language { Id = "ar_i", Name = LanguageName.Arabic, Target = LanguageTarget.Invoice },
                new Language { Id = "en_i", Name = LanguageName.English, Target = LanguageTarget.Invoice }
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
        }
    }
}
