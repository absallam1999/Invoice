using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class AddTabAccountIdToPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TabAccountId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TabAccountIds",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6352));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6448));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6942));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6893));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6832));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6870));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6881));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(7072));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6953));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6964));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6904));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(7051));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6975));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6915));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "tp",
                column: "CreatedAt",
                value: new DateTime(2025, 11, 1, 4, 34, 45, 898, DateTimeKind.Unspecified).AddTicks(6931));
        }
    }
}
