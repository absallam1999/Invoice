using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class tax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Invoices");

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "Target", "UpdatedAt" },
                values: new object[,]
                {
                    { "ar", new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(854), null, "Arabic", "Page", null },
                    { "en", new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(910), null, "English", "Page", null }
                });

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { "bt", new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(1216), null, "BankTransfer", null },
                    { "ca", new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(1190), null, "Cash", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar");

            migrationBuilder.DeleteData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca");

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Invoices",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
