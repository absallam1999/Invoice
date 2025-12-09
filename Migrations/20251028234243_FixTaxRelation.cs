using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class FixTaxRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5074));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5209));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5667));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5637));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5574));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5616));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5627));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5719));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5677));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5687));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5647));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5710));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5700));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 42, 41, 419, DateTimeKind.Unspecified).AddTicks(5657));

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(3887));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(3949));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4360));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4312));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4262));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4293));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4303));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4404));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4369));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4378));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4342));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4395));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4386));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 2, 13, 30, 167, DateTimeKind.Unspecified).AddTicks(4352));

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id");
        }
    }
}
