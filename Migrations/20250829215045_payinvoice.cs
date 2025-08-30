using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class payinvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayInvoices_InvoiceId",
                table: "PayInvoices");

            //migrationBuilder.UpdateData(
            //    table: "Languages",
            //    keyColumn: "Id",
            //    keyValue: "ar",
            //    column: "CreatedAt",
            //    value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5460));

            //migrationBuilder.UpdateData(
            //    table: "Languages",
            //    keyColumn: "Id",
            //    keyValue: "en",
            //    column: "CreatedAt",
            //    value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5472));

            //migrationBuilder.UpdateData(
            //    table: "PaymentMethods",
            //    keyColumn: "Id",
            //    keyValue: "bt",
            //    column: "CreatedAt",
            //    value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5736));

            //migrationBuilder.UpdateData(
            //    table: "PaymentMethods",
            //    keyColumn: "Id",
            //    keyValue: "ca",
            //    column: "CreatedAt",
            //    value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5712));

            migrationBuilder.CreateIndex(
                name: "IX_PayInvoices_InvoiceId",
                table: "PayInvoices",
                column: "InvoiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PayInvoices_InvoiceId",
                table: "PayInvoices");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(854));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(910));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(1216));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 18, 4, 620, DateTimeKind.Utc).AddTicks(1190));

            migrationBuilder.CreateIndex(
                name: "IX_PayInvoices_InvoiceId",
                table: "PayInvoices",
                column: "InvoiceId");
        }
    }
}
