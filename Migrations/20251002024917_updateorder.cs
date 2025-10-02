using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updateorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StoreId",
                table: "Orders",
                type: "nvarchar(8)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StoreId",
                table: "Orders",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Stores_StoreId",
                table: "Orders",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Stores_StoreId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StoreId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6036));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6078));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6461));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6431));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6385));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6413));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6422));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6514));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6470));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6484));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6441));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6504));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6494));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 29, 4, 56, 59, 264, DateTimeKind.Unspecified).AddTicks(6450));
        }
    }
}
