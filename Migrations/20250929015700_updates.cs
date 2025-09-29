using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_PaymentLinks_PaymentLinkId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PaymentLinkId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentLinkId",
                table: "Invoices");

            migrationBuilder.CreateTable(
                name: "PaymentLinkPayments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaymentsNumber = table.Column<int>(type: "int", nullable: false),
                    PaymentType = table.Column<int>(type: "int", nullable: false),
                    PaymentLinkId = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    InvoiceId = table.Column<string>(type: "nvarchar(8)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentLinkPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentLinkPayments_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentLinkPayments_PaymentLinks_PaymentLinkId",
                        column: x => x.PaymentLinkId,
                        principalTable: "PaymentLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinkPayments_InvoiceId",
                table: "PaymentLinkPayments",
                column: "InvoiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinkPayments_PaymentLinkId",
                table: "PaymentLinkPayments",
                column: "PaymentLinkId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentLinkPayments");

            migrationBuilder.AddColumn<string>(
                name: "PaymentLinkId",
                table: "Invoices",
                type: "nvarchar(8)",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentLinkId",
                table: "Invoices",
                column: "PaymentLinkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_PaymentLinks_PaymentLinkId",
                table: "Invoices",
                column: "PaymentLinkId",
                principalTable: "PaymentLinks",
                principalColumn: "Id");
        }
    }
}
