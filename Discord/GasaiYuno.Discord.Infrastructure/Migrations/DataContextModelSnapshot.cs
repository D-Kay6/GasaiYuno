﻿// <auto-generated />
using System;
using GasaiYuno.Discord.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("Latin1_General_CI_AS")
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.HasSequence("languageseq")
                .IncrementsBy(10);

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Ban", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("User")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Reason")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.HasKey("ServerId", "User");

                    b.ToTable("Bans", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.CustomCommand", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Command")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.HasKey("ServerId", "Command");

                    b.ToTable("CustomCommands", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.DynamicChannel", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Channels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GeneratedChannels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GenerationName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)")
                        .HasDefaultValue("-- channel");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServerId", "Name");

                    b.ToTable("DynamicChannels", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "languageseq");

                    b.Property<string>("LocalizedName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Languages", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Notification", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal?>("Channel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Image")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Message")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)")
                        .HasDefaultValue("Welcome to the party {0}. Hope you will have a good time with us.");

                    b.HasKey("ServerId", "Type");

                    b.ToTable("Notifications", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Poll", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("Channel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("Message")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("MultiSelect")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Options")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("ServerId", "Channel", "Message");

                    b.ToTable("Polls", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Raffle", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("Channel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("Message")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("Users")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServerId", "Channel", "Message");

                    b.ToTable("Raffles", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Server", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int?>("LanguageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(1);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasDefaultValue("!");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("Servers", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Ban", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.CustomCommand", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.DynamicChannel", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Notification", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Poll", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Raffle", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Server", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Language", "Language")
                        .WithMany()
                        .HasForeignKey("LanguageId");

                    b.Navigation("Language");
                });
#pragma warning restore 612, 618
        }
    }
}
