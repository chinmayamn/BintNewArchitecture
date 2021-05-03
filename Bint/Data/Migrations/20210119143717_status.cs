using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bint.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class status : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "transferusd",
                newName: "ToTotalAmount");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transferusd",
                newName: "ToStatus");

            migrationBuilder.AddColumn<string>(
                name: "FromStatus",
                table: "transferusd",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FromTotalAmount",
                table: "transferusd",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromStatus",
                table: "transferusd");

            migrationBuilder.DropColumn(
                name: "FromTotalAmount",
                table: "transferusd");

            migrationBuilder.RenameColumn(
                name: "ToTotalAmount",
                table: "transferusd",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "ToStatus",
                table: "transferusd",
                newName: "Status");
        }
    }
}
