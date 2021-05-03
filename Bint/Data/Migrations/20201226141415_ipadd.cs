using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bint.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class ipadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IP",
                table: "CaptureDeviceData",
                newName: "IPv6");

            migrationBuilder.AddColumn<string>(
                name: "IPv4",
                table: "CaptureDeviceData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPv4",
                table: "CaptureDeviceData");

            migrationBuilder.RenameColumn(
                name: "IPv6",
                table: "CaptureDeviceData",
                newName: "IP");
        }
    }
}
