using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatepayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6324));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6430));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7015));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6953));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6889));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6929));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6941));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7155));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7027));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7038));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6979));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7129));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7050));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(6991));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "tp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 31, 3, 43, 31, 755, DateTimeKind.Unspecified).AddTicks(7003));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6143));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6200));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6746));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6710));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6635));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6670));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6698));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6811));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6758));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6770));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6723));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6799));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6782));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 29, 3, 1, 20, 718, DateTimeKind.Unspecified).AddTicks(6735));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "tp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 27, 23, 34, 26, 827, DateTimeKind.Unspecified).AddTicks(7624));
        }
    }
}
