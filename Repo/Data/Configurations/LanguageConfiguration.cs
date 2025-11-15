using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class LanguageConfiguration : BaseEntityConfiguration<Language>
    {
        public override void Configure(EntityTypeBuilder<Language> builder)
        {
            base.Configure(builder);

            builder.ToTable("Languages");

            builder.Property(l => l.Name)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            builder.Property(l => l.Target)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(20);

            //builder.HasMany(l => l.Pages)
            //       .WithOne(p => p.Language)
            //       .HasForeignKey(p => p.LanguageId)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(l => l.Stores)
            //       .WithOne(s => s.Language)
            //       .HasForeignKey(s => s.LanguageId)
            //       .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(l => l.Invoices)
            //       .WithOne(i => i.Language)
            //       .HasForeignKey(i => i.LanguageId)
                   //.OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(l => new { l.Name, l.Target })
                   .IsUnique();
        }
    }
}
