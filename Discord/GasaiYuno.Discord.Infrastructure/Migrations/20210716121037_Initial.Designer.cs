﻿// <auto-generated />
using System;
using GasaiYuno.Discord.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace GasaiYuno.Discord.Infrastructure.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210716121037_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:Collation", "Latin1_General_CI_AS")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.8")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.ToTable("Bans");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.CustomCommand", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Command")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Response")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)");

                    b.HasKey("ServerId", "Command");

                    b.ToTable("CustomCommands");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.DynamicChannel", b =>
                {
                    b.Property<decimal>("ServerId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("Name")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Channels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GeneratedChannels")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GenerationName")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasDefaultValue("-- channel");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ServerId", "Name");

                    b.ToTable("DynamicChannels");
                });

            modelBuilder.Entity("GasaiYuno.Discord.Domain.Language", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:HiLoSequenceName", "languageseq")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.SequenceHiLo);

                    b.Property<string>("LocalizedName")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Languages");
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
                        .HasMaxLength(2000)
                        .HasColumnType("nvarchar(2000)")
                        .HasDefaultValue("Welcome to the party {0}. Hope you will have a good time with us.");

                    b.HasKey("ServerId", "Type");

                    b.ToTable("Notifications");
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
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Prefix")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasDefaultValue("!");

                    b.HasKey("Id");

                    b.HasIndex("LanguageId");

                    b.ToTable("Servers");
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
