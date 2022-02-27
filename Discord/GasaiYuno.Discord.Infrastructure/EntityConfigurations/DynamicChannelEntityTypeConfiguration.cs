using GasaiYuno.Discord.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class DynamicChannelEntityTypeConfiguration : IEntityTypeConfiguration<DynamicChannel>
    {
        public void Configure(EntityTypeBuilder<DynamicChannel> builder)
        {
            builder.ToTable("DynamicChannels");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            builder.Property(x => x.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.GenerationName)
                .HasMaxLength(200)
                .HasDefaultValue("-- channel")
                .IsRequired();

            builder.Property(x => x.Channels)
                .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<ulong>>(x));

            builder.Property(x => x.GeneratedChannels)
                .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<ulong>>(x));

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasKey("ServerId", nameof(DynamicChannel.Name));
        }
    }
}