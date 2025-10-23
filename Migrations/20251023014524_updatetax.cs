using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatetax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<string>(
                name: "TaxId",
                table: "Invoices",
                type: "nvarchar(450)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_TaxId",
                table: "Invoices",
                column: "TaxId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices",
                column: "TaxId",
                principalTable: "Taxes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Taxes_TaxId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_TaxId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "MainImage",
                table: "Products",
                newName: "Image");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(6185));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(6283));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7344));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7258));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7126));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7208));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7234));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7502));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7369));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7392));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7280));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7478));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7449));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 10, 2, 5, 49, 16, 112, DateTimeKind.Unspecified).AddTicks(7320));
        }
    }
}
