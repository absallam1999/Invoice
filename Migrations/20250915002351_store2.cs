using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class store2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Stores_Languages_LanguageId",
            //    table: "Stores");

            //migrationBuilder.DropIndex(
            //    name: "IX_Stores_LanguageId",
            //    table: "Stores");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Shipping_PaymentType",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "Tax",
                table: "Stores",
                newName: "PaymentOptions_Tax");

            migrationBuilder.AddColumn<bool>(
                name: "PaymentOptions_BankTransfer",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentOptions_Cash",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentOptions_PayPal",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "StoreSettings_purchaseOptions_Address",
                table: "Stores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Currencies_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 0, 23, 49, 986, DateTimeKind.Utc).AddTicks(3227));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 0, 23, 49, 986, DateTimeKind.Utc).AddTicks(3241));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 0, 23, 49, 986, DateTimeKind.Utc).AddTicks(3581));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 0, 23, 49, 986, DateTimeKind.Utc).AddTicks(3554));

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_UserId",
                table: "Currencies",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropColumn(
                name: "PaymentOptions_BankTransfer",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PaymentOptions_Cash",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PaymentOptions_PayPal",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "StoreSettings_purchaseOptions_Address",
                table: "Stores");

            migrationBuilder.RenameColumn(
                name: "PaymentOptions_Tax",
                table: "Stores",
                newName: "Tax");

            migrationBuilder.AddColumn<string>(
                name: "LanguageId",
                table: "Stores",
                type: "nvarchar(8)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentMethod",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Shipping_PaymentType",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                name: "IX_Stores_LanguageId",
                table: "Stores",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Languages_LanguageId",
                table: "Stores",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
