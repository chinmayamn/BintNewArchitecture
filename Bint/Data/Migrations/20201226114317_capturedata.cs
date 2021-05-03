using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bint.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class capturedata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaptureDeviceData",
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
                    IP = table.Column<string>(nullable: true),
                    LoginTime = table.Column<DateTime>(nullable: false),
                    LogoutTime = table.Column<DateTime>(nullable: false),
                    OSName = table.Column<string>(nullable: true),
                    OSPlatform = table.Column<string>(nullable: true),
                    OSVersion = table.Column<string>(nullable: true),
                    Useragent = table.Column<string>(nullable: true),
                    userid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__captureDeviceData", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaptureDeviceData");
        }
    }
}
