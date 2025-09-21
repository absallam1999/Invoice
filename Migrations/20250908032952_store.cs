using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class store : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactInformation_Stores_StoreId",
                table: "ContactInformation");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Languages_LanguageId",
                table: "Pages");

            //migrationBuilder.DropIndex(
            //    name: "IX_Stores_UserId",
            //    table: "Stores");

            //migrationBuilder.DropIndex(
            //    name: "IX_Pages_LanguageId",
            //    table: "Pages");

            //migrationBuilder.DropIndex(
            //    name: "IX_ContactInformation_StoreId",
            //    table: "ContactInformation");

            migrationBuilder.DropColumn(
                name: "StoreSettings_Url",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Pages");

            migrationBuilder.AddColumn<int>(
                name: "ParallelMergeOptions",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Shipping_Delivery",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreSettings_Country",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Pages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 3, 29, 51, 214, DateTimeKind.Utc).AddTicks(357));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 3, 29, 51, 214, DateTimeKind.Utc).AddTicks(377));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 3, 29, 51, 214, DateTimeKind.Utc).AddTicks(870));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 8, 3, 29, 51, 214, DateTimeKind.Utc).AddTicks(833));

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UserId",
                table: "Stores",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_StoreId",
                table: "ContactInformation",
                column: "StoreId",
                unique: true,
                filter: "[StoreId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactInformation_Stores_StoreId",
                table: "ContactInformation",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactInformation_Stores_StoreId",
                table: "ContactInformation");

            migrationBuilder.DropIndex(
                name: "IX_Stores_UserId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_ContactInformation_StoreId",
                table: "ContactInformation");

            migrationBuilder.DropColumn(
                name: "ParallelMergeOptions",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Shipping_Delivery",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreSettings_Country",
                table: "Stores");

            migrationBuilder.AddColumn<string>(
                name: "StoreSettings_Url",
                table: "Stores",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Pages",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                table: "Pages",
                type: "nvarchar(8)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5460));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5472));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5736));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 8, 29, 21, 50, 44, 700, DateTimeKind.Utc).AddTicks(5712));

            migrationBuilder.CreateIndex(
                name: "IX_Stores_UserId",
                table: "Stores",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LanguageId",
                table: "Pages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactInformation_StoreId",
                table: "ContactInformation",
                column: "StoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactInformation_Stores_StoreId",
                table: "ContactInformation",
                column: "StoreId",
                principalTable: "Stores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Languages_LanguageId",
                table: "Pages",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
