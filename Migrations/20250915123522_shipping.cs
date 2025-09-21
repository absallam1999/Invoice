using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class shipping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Shipping_DeliveryFee",
                table: "Stores",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Shipping_Region",
                table: "Stores",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    ShippingMethod = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: true),
                    DeliveryCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 12, 35, 21, 544, DateTimeKind.Utc).AddTicks(5067));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 12, 35, 21, 544, DateTimeKind.Utc).AddTicks(5084));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 12, 35, 21, 544, DateTimeKind.Utc).AddTicks(5453));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 15, 12, 35, 21, 544, DateTimeKind.Utc).AddTicks(5424));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_InvoiceId",
                table: "Orders",
                column: "InvoiceId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropColumn(
                name: "Shipping_DeliveryFee",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Shipping_Region",
                table: "Stores");

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
        }
    }
}
