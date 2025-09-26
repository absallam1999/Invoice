using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatepaymentlink2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpireDate",
                table: "PaymentLinks",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpireDate",
                table: "PaymentLinks",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(290));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(324));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(661));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(639));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(587));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(623));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(631));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(701));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(669));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(676));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(646));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(693));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(686));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 26, 2, 12, 27, 940, DateTimeKind.Unspecified).AddTicks(654));
        }
    }
}
