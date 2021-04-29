﻿// <auto-generated />
using Bint.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Bint.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210113170852_activitylog")]
    partial class activitylog
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bint.Models.ActivityLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Activity");

                    b.Property<DateTime>("ActivityDate");

                    b.Property<string>("Userid");

                    b.HasKey("Id");

                    b.ToTable("activitylog");
                });

            modelBuilder.Entity("Bint.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("Account_holder_name");

                    b.Property<string>("Address");

                    b.Property<decimal>("BGC");

                    b.Property<string>("Bankaccount");

                    b.Property<string>("Bankname");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Created_by");

                    b.Property<string>("Created_id");

                    b.Property<DateTime>("Created_on");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Firstname");

                    b.Property<string>("Ifsc_code");

                    b.Property<string>("Lastname");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Mobile");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("OTP");

                    b.Property<string>("Pan");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("Profilepicture");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Status");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<decimal>("USD");

                    b.Property<string>("Upi_id");

                    b.Property<string>("UserId");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Bint.Models.CaptureDeviceData", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand");

                    b.Property<string>("BrandName");

                    b.Property<string>("BrowserName");

                    b.Property<string>("BrowserVersion");

                    b.Property<string>("DeviceModel");

                    b.Property<string>("DeviceName");

                    b.Property<string>("IPv4");

                    b.Property<string>("IPv6");

                    b.Property<DateTime>("LoginTime");

                    b.Property<DateTime>("LogoutTime");

                    b.Property<string>("OSName");

                    b.Property<string>("OSPlatform");

                    b.Property<string>("OSVersion");

                    b.Property<string>("PublicIp");

                    b.Property<string>("Useragent");

                    b.Property<string>("urole");

                    b.Property<string>("userid");

                    b.HasKey("id");

                    b.ToTable("_captureDeviceData");
                });

            modelBuilder.Entity("Bint.Models.Doc", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Docpath");

                    b.Property<string>("Filename");

                    b.Property<string>("Reason");

                    b.Property<string>("Status");

                    b.Property<string>("Userid");

                    b.HasKey("Id");

                    b.ToTable("Doc");
                });

            modelBuilder.Entity("Bint.Models.ErrorLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Exception");

                    b.Property<string>("Level");

                    b.Property<string>("Message");

                    b.Property<string>("MessageTemplate");

                    b.Property<string>("Properties");

                    b.Property<DateTime>("TimeStamp");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("Bint.Models.RegId", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdminId");

                    b.Property<string>("ClientId");

                    b.Property<string>("InvestorId");

                    b.Property<string>("PartnerId");

                    b.HasKey("Id");

                    b.ToTable("regId");
                });

            modelBuilder.Entity("Bint.Models.RestrictedAccess", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand");

                    b.Property<string>("BrandName");

                    b.Property<string>("BrowserName");

                    b.Property<string>("BrowserVersion");

                    b.Property<string>("DeviceModel");

                    b.Property<string>("DeviceName");

                    b.Property<DateTime>("ErrorTime");

                    b.Property<string>("IPv4");

                    b.Property<string>("IPv6");

                    b.Property<string>("OSName");

                    b.Property<string>("OSPlatform");

                    b.Property<string>("OSVersion");

                    b.Property<string>("PublicIp");

                    b.Property<string>("ReturnUrl");

                    b.Property<string>("Useragent");

                    b.Property<string>("Verified");

                    b.Property<string>("urole");

                    b.Property<string>("userid");

                    b.HasKey("id");

                    b.ToTable("_restrictedAccess");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Bint.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Bint.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bint.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Bint.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
