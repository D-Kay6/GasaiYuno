using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class RaffleEntityTypeConfiguration : IEntityTypeConfiguration<Raffle>
    {
        public void Configure(EntityTypeBuilder<Raffle> builder)
        {
            builder.ToTable("Raffles");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Channel)
                .IsRequired();

            builder.Property(x => x.Message)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.Users)
                .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<ulong>>(x));

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey("ServerId", nameof(Raffle.Channel), nameof(Raffle.Message));
        }
    }
}