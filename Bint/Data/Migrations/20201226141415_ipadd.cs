using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bint.Data.Migrations
{
    public partial class ipadd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IP",
                table: "_captureDeviceData",
                newName: "IPv6");

            migrationBuilder.AddColumn<string>(
                name: "IPv4",
                table: "_captureDeviceData",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPv4",
                table: "_captureDeviceData");

            migrationBuilder.RenameColumn(
                name: "IPv6",
                table: "_captureDeviceData",
                newName: "IP");
        }
    }
}
