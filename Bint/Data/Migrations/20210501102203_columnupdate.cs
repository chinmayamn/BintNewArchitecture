using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Bint.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class columnupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_transferusd",
                table: "transferusd");

            migrationBuilder.DropPrimaryKey(
                name: "PK_regId",
                table: "regId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_depositwithdraw",
                table: "depositwithdraw");

            migrationBuilder.DropPrimaryKey(
                name: "PK_activitylog",
                table: "activitylog");

            migrationBuilder.RenameTable(
                name: "transferusd",
                newName: "TransferUsd");

            migrationBuilder.RenameTable(
                name: "regId",
                newName: "RegId");

            migrationBuilder.RenameTable(
                name: "depositwithdraw",
                newName: "DepositWithdraw");

            migrationBuilder.RenameTable(
                name: "activitylog",
                newName: "ActivityLog");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "RestrictedAccess",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "urole",
                table: "RestrictedAccess",
                newName: "Urole");

            migrationBuilder.RenameColumn(
                name: "OSVersion",
                table: "RestrictedAccess",
                newName: "OsVersion");

            migrationBuilder.RenameColumn(
                name: "OSPlatform",
                table: "RestrictedAccess",
                newName: "OsPlatform");

            migrationBuilder.RenameColumn(
                name: "OSName",
                table: "RestrictedAccess",
                newName: "OsName");

            migrationBuilder.RenameColumn(
                name: "IPv6",
                table: "RestrictedAccess",
                newName: "Ipv6");

            migrationBuilder.RenameColumn(
                name: "IPv4",
                table: "RestrictedAccess",
                newName: "Ipv4");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RestrictedAccess",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "Docpath",
                table: "Doc",
                newName: "DocPath");

            migrationBuilder.RenameColumn(
                name: "USDAction",
                table: "DepositWithdraw",
                newName: "UsdAction");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "CaptureDeviceData",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "urole",
                table: "CaptureDeviceData",
                newName: "URole");

            migrationBuilder.RenameColumn(
                name: "OSVersion",
                table: "CaptureDeviceData",
                newName: "OsVersion");

            migrationBuilder.RenameColumn(
                name: "OSPlatform",
                table: "CaptureDeviceData",
                newName: "OsPlatform");

            migrationBuilder.RenameColumn(
                name: "OSName",
                table: "CaptureDeviceData",
                newName: "OsName");

            migrationBuilder.RenameColumn(
                name: "IPv6",
                table: "CaptureDeviceData",
                newName: "Ipv6");

            migrationBuilder.RenameColumn(
                name: "IPv4",
                table: "CaptureDeviceData",
                newName: "Ipv4");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CaptureDeviceData",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "QRCode",
                table: "AspNetUsers",
                newName: "QrCode");

            migrationBuilder.RenameColumn(
                name: "PartnerQRCode",
                table: "AspNetUsers",
                newName: "PartnerQrCode");

            migrationBuilder.RenameColumn(
                name: "OTP",
                table: "AspNetUsers",
                newName: "Otp");

            migrationBuilder.RenameColumn(
                name: "InvestorQRCode",
                table: "AspNetUsers",
                newName: "InvestorQrCode");

            migrationBuilder.RenameColumn(
                name: "ClientQRCode",
                table: "AspNetUsers",
                newName: "ClientQrCode");

            migrationBuilder.RenameColumn(
                name: "AdminQRCode",
                table: "AspNetUsers",
                newName: "AdminQrCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransferUsd",
                table: "TransferUsd",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RegId",
                table: "RegId",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DepositWithdraw",
                table: "DepositWithdraw",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_TransferUsd",
                table: "TransferUsd");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RegId",
                table: "RegId");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DepositWithdraw",
                table: "DepositWithdraw");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityLog",
                table: "ActivityLog");

            migrationBuilder.RenameTable(
                name: "TransferUsd",
                newName: "transferusd");

            migrationBuilder.RenameTable(
                name: "RegId",
                newName: "regId");

            migrationBuilder.RenameTable(
                name: "DepositWithdraw",
                newName: "depositwithdraw");

            migrationBuilder.RenameTable(
                name: "ActivityLog",
                newName: "activitylog");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "RestrictedAccess",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "Urole",
                table: "RestrictedAccess",
                newName: "urole");

            migrationBuilder.RenameColumn(
                name: "OsVersion",
                table: "RestrictedAccess",
                newName: "OSVersion");

            migrationBuilder.RenameColumn(
                name: "OsPlatform",
                table: "RestrictedAccess",
                newName: "OSPlatform");

            migrationBuilder.RenameColumn(
                name: "OsName",
                table: "RestrictedAccess",
                newName: "OSName");

            migrationBuilder.RenameColumn(
                name: "Ipv6",
                table: "RestrictedAccess",
                newName: "IPv6");

            migrationBuilder.RenameColumn(
                name: "Ipv4",
                table: "RestrictedAccess",
                newName: "IPv4");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RestrictedAccess",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DocPath",
                table: "Doc",
                newName: "Docpath");

            migrationBuilder.RenameColumn(
                name: "UsdAction",
                table: "depositwithdraw",
                newName: "USDAction");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CaptureDeviceData",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "URole",
                table: "CaptureDeviceData",
                newName: "urole");

            migrationBuilder.RenameColumn(
                name: "OsVersion",
                table: "CaptureDeviceData",
                newName: "OSVersion");

            migrationBuilder.RenameColumn(
                name: "OsPlatform",
                table: "CaptureDeviceData",
                newName: "OSPlatform");

            migrationBuilder.RenameColumn(
                name: "OsName",
                table: "CaptureDeviceData",
                newName: "OSName");

            migrationBuilder.RenameColumn(
                name: "Ipv6",
                table: "CaptureDeviceData",
                newName: "IPv6");

            migrationBuilder.RenameColumn(
                name: "Ipv4",
                table: "CaptureDeviceData",
                newName: "IPv4");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CaptureDeviceData",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "QrCode",
                table: "AspNetUsers",
                newName: "QRCode");

            migrationBuilder.RenameColumn(
                name: "PartnerQrCode",
                table: "AspNetUsers",
                newName: "PartnerQRCode");

            migrationBuilder.RenameColumn(
                name: "Otp",
                table: "AspNetUsers",
                newName: "OTP");

            migrationBuilder.RenameColumn(
                name: "InvestorQrCode",
                table: "AspNetUsers",
                newName: "InvestorQRCode");

            migrationBuilder.RenameColumn(
                name: "ClientQrCode",
                table: "AspNetUsers",
                newName: "ClientQRCode");

            migrationBuilder.RenameColumn(
                name: "AdminQrCode",
                table: "AspNetUsers",
                newName: "AdminQRCode");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transferusd",
                table: "transferusd",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_regId",
                table: "regId",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_depositwithdraw",
                table: "depositwithdraw",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_activitylog",
                table: "activitylog",
                column: "Id");
        }
    }
}
