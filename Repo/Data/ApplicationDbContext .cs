using invoice.Core.Entites;
using invoice.Core.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

            //add languages
            builder.Entity<Language>().HasData(
                new Language
                {
                    Id = "ar",
                    Name = LanguageName.Arabic,
                },
                new Language
                {
                    Id = "en",
                    Name = LanguageName.English,
                }
            );

            //add payment methods
            builder.Entity<PaymentMethod>().HasData(
                new PaymentMethod
                {
                    Id = "ca",
                    Name = PaymentType.Cash,
                },
                new PaymentMethod
                {
                    Id = "bt",
                    Name = PaymentType.BankTransfer,

                }
                );





        }
    }
}
