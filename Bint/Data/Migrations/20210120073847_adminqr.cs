using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bint.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class adminqr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminQRCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminTetherAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientQRCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientTetherAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestorQRCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvestorTetherAddress",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerQRCode",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerTetherAddress",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminQRCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdminTetherAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientQRCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ClientTetherAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InvestorQRCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "InvestorTetherAddress",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PartnerQRCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PartnerTetherAddress",
                table: "AspNetUsers");
        }
    }
}
