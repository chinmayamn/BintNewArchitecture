using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bint.Data.Migrations
{
    public partial class rename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoginTime",
                table: "RestrictedAccess");

            migrationBuilder.RenameColumn(
                name: "LogoutTime",
                table: "RestrictedAccess",
                newName: "ErrorTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ErrorTime",
                table: "RestrictedAccess",
                newName: "LogoutTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginTime",
                table: "RestrictedAccess",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
