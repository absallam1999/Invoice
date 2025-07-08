using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using invoice.Models;

namespace invoice.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<PaymentLink> PaymentLinks { get; set; }
        public DbSet<ContactInformation> ContactInformations { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PurchaseCompletionOptions> PurchaseCompletionOptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Invoice>()
                .Property(i => i.InvoiceStatus)
                .HasConversion<string>();

            builder.Entity<Invoice>()
                .Property(i => i.InvoiceType)
                .HasConversion<string>();

            builder.Entity<Invoice>()
                .HasOne(i => i.Payment)
                .WithOne(p => p.Invoice)
                .HasForeignKey<Payment>(p => p.InvoiceId);

            builder.Entity<Payment>()
                .HasOne(p => p.PaymentLink)
                .WithOne(pl => pl.Payment)
                .HasForeignKey<PaymentLink>(pl => pl.PaymentId);

            builder.Entity<Store>()
                .HasOne(s => s.PurchaseCompletionOptions)
                .WithOne(p => p.Store)
                .HasForeignKey<PurchaseCompletionOptions>(p => p.StoreId);

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId);

            builder.Entity<Page>()
                .HasOne(p => p.Language)
                .WithMany(l => l.Pages)
                .HasForeignKey(p => p.LanguageId);

            builder.Entity<Invoice>()
                .HasOne(i => i.Language)
                .WithMany(l => l.Invoices)
                .HasForeignKey(i => i.LanguageId);
        }
    }
}
