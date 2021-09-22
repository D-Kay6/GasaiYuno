using GasaiYuno.Discord.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Infrastructure.EntityConfigurations
{
    public class DynamicRoleEntityTypeConfiguration : IEntityTypeConfiguration<DynamicRole>
    {
        public void Configure(EntityTypeBuilder<DynamicRole> builder)
        {
            builder.ToTable("DynamicRoles");

            builder.Property<ulong>("ServerId")
                .IsRequired();

            //builder.Property(x => x.)
            //    .HasConversion<string>()
            //    .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(200)
                .IsRequired();

            //builder.Property(x => x.Status)
            //    .HasMaxLength(100)
            //    .IsRequired();
            
            builder.Property(x => x.Roles)
                .HasConversion(x => JsonConvert.SerializeObject(x), x => JsonConvert.DeserializeObject<List<ulong>>(x));

            builder.HasOne(x => x.Server)
                .WithMany()
                .HasForeignKey("ServerId")
                .HasPrincipalKey(x => x.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //builder.HasKey("ServerId", nameof(DynamicRole.Type), nameof(DynamicRole.Status));
        }
    }
}