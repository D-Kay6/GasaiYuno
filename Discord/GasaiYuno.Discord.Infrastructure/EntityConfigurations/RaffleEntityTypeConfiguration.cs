using GasaiYuno.Discord.Domain.Models;
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

            builder.Property(x => x.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever()
                .IsRequired();

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

            builder.HasMany(x => x.Entries)
                .WithOne()
                .HasForeignKey("RaffleId")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasKey(x => x.Id);
        }
    }
    public class RaffleEntryEntityTypeConfiguration : IEntityTypeConfiguration<RaffleEntry>
    {
        public void Configure(EntityTypeBuilder<RaffleEntry> builder)
        {
            builder.ToTable("RaffleEntries");

            builder.Property<int>("Id")
                .UseHiLo("raffleentryseq");

            builder.Property<ulong>("RaffleId")
                .IsRequired();

            builder.Property(x => x.User)
                .IsRequired();

            builder.HasKey("Id");
        }
    }
}