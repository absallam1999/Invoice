using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatetax2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Invoices",
                newName: "HaveTax");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HaveTax",
                table: "Invoices",
                newName: "Tax");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 958, DateTimeKind.Unspecified).AddTicks(8896));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 958, DateTimeKind.Unspecified).AddTicks(8987));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(132));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(35));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 958, DateTimeKind.Unspecified).AddTicks(9866));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 958, DateTimeKind.Unspecified).AddTicks(9947));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 958, DateTimeKind.Unspecified).AddTicks(9981));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(467));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(325));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(363));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(68));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(424));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(395));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 23, 4, 45, 22, 959, DateTimeKind.Unspecified).AddTicks(100));
        }
    }
}
