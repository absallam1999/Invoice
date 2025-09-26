using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatepaymentlink3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PaymentLinks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(4520));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(4570));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5031));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5004));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(4901));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(4934));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(4994));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5078));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5040));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5052));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5013));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5069));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5061));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 16, 39, 897, DateTimeKind.Unspecified).AddTicks(5022));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "PaymentLinks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7220));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7254));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7592));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7565));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7539));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7550));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7558));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7629));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7600));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7607));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7573));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7622));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7614));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 3, 10, 22, 375, DateTimeKind.Unspecified).AddTicks(7580));
        }
    }
}
