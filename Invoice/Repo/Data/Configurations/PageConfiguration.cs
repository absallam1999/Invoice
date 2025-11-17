using invoice.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace invoice.Repo.Data.Configurations
{
    public class PageConfiguration : BaseEntityConfiguration<Page>
    {
        public override void Configure(EntityTypeBuilder<Page> builder)
        {
            base.Configure(builder);

            builder.ToTable("Pages");

            builder.Property(p => p.Title)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(p => p.Content)
                   .IsRequired();

            builder.Property(p => p.Image)
                   .HasMaxLength(500);

            builder.Property(p => p.InFooter)
                   .HasDefaultValue(false);

            builder.Property(p => p.InHeader)
                   .HasDefaultValue(false);

            builder.HasOne(p => p.Store)
                   .WithMany(s => s.Pages)
                   .HasForeignKey(p => p.StoreId)
                   .OnDelete(DeleteBehavior.Restrict);

            //builder.HasOne(p => p.Language)
            //       .WithMany(l => l.Pages)
            //       .HasForeignKey(p => p.LanguageId)
            //       .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
