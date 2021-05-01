using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Bint.Data.Migrations
{
    public partial class restrictedaccess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestrictedAccess",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Brand = table.Column<string>(nullable: true),
                    BrandName = table.Column<string>(nullable: true),
                    BrowserName = table.Column<string>(nullable: true),
                    BrowserVersion = table.Column<string>(nullable: true),
                    DeviceModel = table.Column<string>(nullable: true),
                    DeviceName = table.Column<string>(nullable: true),
                    IPv4 = table.Column<string>(nullable: true),
                    IPv6 = table.Column<string>(nullable: true),
                    LoginTime = table.Column<DateTime>(nullable: false),
                    LogoutTime = table.Column<DateTime>(nullable: false),
                    OSName = table.Column<string>(nullable: true),
                    OSPlatform = table.Column<string>(nullable: true),
                    OSVersion = table.Column<string>(nullable: true),
                    PublicIp = table.Column<string>(nullable: true),
                    ReturnUrl = table.Column<string>(nullable: true),
                    Useragent = table.Column<string>(nullable: true),
                    Verified = table.Column<string>(nullable: true),
                    urole = table.Column<string>(nullable: true),
                    userid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__restrictedAccess", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestrictedAccess");
        }
    }
}
