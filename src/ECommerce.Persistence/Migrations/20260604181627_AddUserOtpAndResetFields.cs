using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOtpAndResetFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationOtp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailConfirmationOtpExpiration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOtpAttemptAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OtpAttempts",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PasswordResetOtp",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PasswordResetOtpExpiration",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationOtp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationOtpExpiration",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastOtpAttemptAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OtpAttempts",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtp",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PasswordResetOtpExpiration",
                table: "AspNetUsers");
        }
    }
}
