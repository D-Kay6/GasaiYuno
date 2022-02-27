﻿// <auto-generated />
using System;
using GasaiYuno.Discord.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220301101550_WarningDisabling")]
    partial class WarningDisabling
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseCollation("Latin1_General_CI_AS")
                .HasAnnotation("ProductVersion", "6.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.HasSequence("raffleentryseq")
                .IncrementsBy(10);

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Ban", b =>
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

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.CustomCommand", b =>
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

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.DynamicChannel", b =>
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

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Notification", b =>
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

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Poll", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)")
                        .HasColumnName("Id");

                    b.Property<decimal>("Channel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Message")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Polls", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.PollOption", b =>
                {
                    b.Property<decimal>("PollId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Value")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("PollId", "Value");

                    b.ToTable("PollOptions", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.PollSelection", b =>
                {
                    b.Property<decimal>("PollId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("User")
                        .HasColumnType("decimal(20,0)");

                    b.Property<int>("SelectedOption")
                        .HasColumnType("int");

                    b.HasKey("PollId", "User", "SelectedOption");

                    b.ToTable("PollSelections", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Raffle", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)")
                        .HasColumnName("Id");

                    b.Property<decimal>("Channel")
                        .HasColumnType("decimal(20,0)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("Message")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Raffles", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.RaffleEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseHiLo(b.Property<int>("Id"), "raffleentryseq");

                    b.Property<decimal>("RaffleId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("User")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("Id");

                    b.HasIndex("RaffleId");

                    b.ToTable("RaffleEntries", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Server", b =>
                {
                    b.Property<decimal>("Id")
                        .HasColumnType("decimal(20,0)")
                        .HasColumnName("Id");

                    b.Property<int>("Language")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

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

                    b.Property<bool>("WarningDisabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.HasKey("Id");

                    b.ToTable("Servers", (string)null);
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Ban", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.CustomCommand", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.DynamicChannel", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Notification", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Poll", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.PollOption", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Poll", null)
                        .WithMany("Options")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.PollSelection", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Poll", null)
                        .WithMany("Selections")
                        .HasForeignKey("PollId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Raffle", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.RaffleEntry", b =>
                {
                    b.HasOne("GasaiYuno.Discord.Domain.Models.Raffle", null)
                        .WithMany("Entries")
                        .HasForeignKey("RaffleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Poll", b =>
                {
                    b.Navigation("Options");

                    b.Navigation("Selections");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Models.Raffle", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
