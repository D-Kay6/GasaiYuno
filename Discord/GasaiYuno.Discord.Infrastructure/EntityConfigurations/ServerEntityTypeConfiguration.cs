using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class ServerEntityTypeConfiguration : IEntityTypeConfiguration<Server>
    {
        public void Configure(EntityTypeBuilder<Server> builder)
        {
            builder.ToTable("Servers");

            builder.Property(x => x.Id)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property<int?>("LanguageId")
                .HasDefaultValue(1)
                .IsRequired(false);

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.Prefix)
                .HasDefaultValue("!")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(x => x.Language)
                .WithMany()
                .HasForeignKey("LanguageId")
                .HasPrincipalKey(x => x.Id)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasKey(x => x.Id);
        }
    }
}