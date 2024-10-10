﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TopUpAPI.DataAccess;

#nullable disable

namespace TopUpAPI.Migrations
{
    [DbContext(typeof(TopUpDbContext))]
    partial class TopUpDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MobileRecharge.Domain.Models.Login", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("UserLogin");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Password = "$2a$11$S6gF2hkeXnxB9U1qKbIu/.2dbhnVJs.7fKy8frGY89qUrNVJ.j9z.",
                            Username = "Prakash"
                        },
                        new
                        {
                            Id = 2,
                            Password = "$2a$11$0Y2jKEAHz7269z2MIm3f9.kYMj3mSN9FBhGwwLdsOKevpmWrVMXwO",
                            Username = "Harini"
                        });
                });

            modelBuilder.Entity("TopUpAPI.Models.Beneficiary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("MonthlyTopUpLimit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Nickname")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Beneficiaries");

                    b.HasData(
                        new
                        {
                            Id = 11,
                            MonthlyTopUpLimit = 500m,
                            Nickname = "William",
                            UserId = 201
                        },
                        new
                        {
                            Id = 12,
                            MonthlyTopUpLimit = 1000m,
                            Nickname = "Robert",
                            UserId = 202
                        });
                });

            modelBuilder.Entity("TopUpAPI.Models.BeneficiaryTopUpDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("BeneficiaryId")
                        .HasColumnType("int");

                    b.Property<decimal>("Charge")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("MonthWise")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("YearWise")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BeneficiaryId");

                    b.ToTable("BeneficiaryTopUpDetails");

                    b.HasData(
                        new
                        {
                            Id = 121,
                            Amount = 100m,
                            BeneficiaryId = 11,
                            Charge = 1m,
                            MonthWise = 10,
                            UserId = 201,
                            YearWise = 2024
                        },
                        new
                        {
                            Id = 122,
                            Amount = 75m,
                            BeneficiaryId = 12,
                            Charge = 1m,
                            MonthWise = 10,
                            UserId = 202,
                            YearWise = 2024
                        });
                });

            modelBuilder.Entity("TopUpAPI.Models.TopUpOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("TopUpOptions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Amount = 5m
                        },
                        new
                        {
                            Id = 2,
                            Amount = 10m
                        },
                        new
                        {
                            Id = 3,
                            Amount = 20m
                        },
                        new
                        {
                            Id = 4,
                            Amount = 30m
                        },
                        new
                        {
                            Id = 5,
                            Amount = 50m
                        },
                        new
                        {
                            Id = 6,
                            Amount = 75m
                        },
                        new
                        {
                            Id = 7,
                            Amount = 100m
                        });
                });

            modelBuilder.Entity("TopUpAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AvailableBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("bit");

                    b.Property<decimal>("TotalTopUpLimit")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 201,
                            AvailableBalance = 1450m,
                            IsVerified = true,
                            TotalTopUpLimit = 3000m,
                            Username = "Prakash"
                        },
                        new
                        {
                            Id = 202,
                            AvailableBalance = 2000m,
                            IsVerified = false,
                            TotalTopUpLimit = 3000m,
                            Username = "Pavithra"
                        });
                });

            modelBuilder.Entity("TopUpAPI.Models.Beneficiary", b =>
                {
                    b.HasOne("TopUpAPI.Models.User", null)
                        .WithMany("Beneficiaries")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TopUpAPI.Models.BeneficiaryTopUpDetails", b =>
                {
                    b.HasOne("TopUpAPI.Models.Beneficiary", null)
                        .WithMany("BeneficiaryTopUp")
                        .HasForeignKey("BeneficiaryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TopUpAPI.Models.Beneficiary", b =>
                {
                    b.Navigation("BeneficiaryTopUp");
                });

            modelBuilder.Entity("TopUpAPI.Models.User", b =>
                {
                    b.Navigation("Beneficiaries");
                });
#pragma warning restore 612, 618
        }
    }
}
