using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentLinkFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_PaymentLinks_Invoices_InvoiceId",
            //    table: "PaymentLinks");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Payments_PaymentLinks_PaymentLinkId",
            //    table: "Payments");

            //migrationBuilder.DropIndex(
            //    name: "IX_Payments_PaymentLinkId",
            //    table: "Payments");

            //migrationBuilder.DropIndex(
            //    name: "IX_PaymentLinks_InvoiceId",
            //    table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentLinkId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "PaymentLinks");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Payments",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Payments",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "DATEADD(DAY, 3, GETUTCDATE())");

            migrationBuilder.AddColumn<string>(
                name: "GatewaySessionId",
                table: "Payments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "Payments",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceId",
                table: "PaymentLinks",
                type: "nvarchar(8)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PaymentLinks",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PaymentLinks",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "PaymentLinks",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            //migrationBuilder.AlterColumn<bool>(
            //    name: "IsDeleted",
            //    table: "Orders",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false,
            //    oldClrType: typeof(bool),
            //    oldType: "bit");

            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "CreatedAt",
            //    table: "Orders",
            //    type: "datetime2",
            //    nullable: false,
            //    defaultValueSql: "GETUTCDATE()",
            //    oldClrType: typeof(DateTime),
            //    oldType: "datetime2");

            //migrationBuilder.AlterColumn<string>(
            //    name: "Id",
            //    table: "Orders",
            //    type: "nvarchar(8)",
            //    maxLength: 8,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LineTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, computedColumnSql: "[Quantity] * [UnitPrice]", stored: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    //table.PrimaryKey("PK_OrderItems", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_OrderItems_Orders_OrderId",
                    //    column: x => x.OrderId,
                    //    principalTable: "Orders",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Cascade);
                    //table.ForeignKey(
                    //    name: "FK_OrderItems_Products_ProductId",
                    //    column: x => x.ProductId,
                    //    principalTable: "Products",
                    //    principalColumn: "Id",
                    //    onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5319));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5329));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5753));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5716));

            migrationBuilder.InsertData(
                table: "PaymentMethods",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { "ap", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5774), null, "ApplePay", null },
                    { "cc", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5728), null, "CreditCard", null },
                    { "dc", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5735), null, "DebitCard", null },
                    { "dl", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5807), null, "Delivery", null },
                    { "gp", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5780), null, "GooglePay", null },
                    { "ma", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5786), null, "Mada", null },
                    { "pp", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5761), null, "PayPal", null },
                    { "sa", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5798), null, "Sadad", null },
                    { "sp", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5792), null, "STCPay", null },
                    { "st", new DateTime(2025, 9, 22, 0, 52, 46, 867, DateTimeKind.Utc).AddTicks(5768), null, "Stripe", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_GatewaySessionId",
                table: "Payments",
                column: "GatewaySessionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinks_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Clients_Email",
            //    table: "Clients",
            //    column: "Email",
            //    unique: true,
            //    filter: "[Email] IS NOT NULL");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Clients_PhoneNumber",
            //    table: "Clients",
            //    column: "PhoneNumber",
            //    unique: true,
            //    filter: "[PhoneNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLinks_Invoices_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLinks_Invoices_InvoiceId",
                table: "PaymentLinks");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropIndex(
                name: "IX_Payments_GatewaySessionId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_PaymentLinks_InvoiceId",
                table: "PaymentLinks");

            migrationBuilder.DropIndex(
                name: "IX_Clients_Email",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Clients_PhoneNumber",
                table: "Clients");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp");

            migrationBuilder.DeleteData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "GatewaySessionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Link",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "PaymentLinks");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Payments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkId",
                table: "Payments",
                type: "nvarchar(8)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceId",
                table: "PaymentLinks",
                type: "nvarchar(8)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "PaymentLinks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8);

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 6, 18, 32, 5, DateTimeKind.Utc).AddTicks(3029));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 6, 18, 32, 5, DateTimeKind.Utc).AddTicks(3046));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 6, 18, 32, 5, DateTimeKind.Utc).AddTicks(3399));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 18, 6, 18, 32, 5, DateTimeKind.Utc).AddTicks(3365));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_PaymentLinkId",
                table: "Payments",
                column: "PaymentLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinks_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLinks_Invoices_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_PaymentLinks_PaymentLinkId",
                table: "Payments",
                column: "PaymentLinkId",
                principalTable: "PaymentLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
