using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class LanguageEntityTypeConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.ToTable("Languages");

            builder.Property(x => x.Id)
                .UseHiLo("languageseq");

            builder.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(x => x.LocalizedName)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasKey(x => x.Id);
        }
    }
}