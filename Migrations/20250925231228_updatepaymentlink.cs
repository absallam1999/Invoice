using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace invoice.Migrations
{
    /// <inheritdoc />
    public partial class updatepaymentlink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLinks_Invoices_InvoiceId",
                table: "PaymentLinks");

            //migrationBuilder.DropIndex(
            //    name: "IX_PaymentLinks_InvoiceId",
            //    table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Purpose",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Terms",
                table: "PaymentLinks");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "PaymentLinks",
                newName: "ClientPhone");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInformations_WhatsApp",
                table: "Stores",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PaymentsNumber",
                table: "PaymentLinks",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "BankTransfer",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "Cash",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClientAddress",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClientEmail",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "ClientName",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExpireDate",
                table: "PaymentLinks",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxPaymentsNumber",
                table: "PaymentLinks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PayPal",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentOptions_Tax",
                table: "PaymentLinks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "PaymentLinks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TermsAndConditions",
                table: "PaymentLinks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PaymentLinks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinks_UserId",
                table: "PaymentLinks",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLinks_AspNetUsers_UserId",
                table: "PaymentLinks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_PaymentLinks_PaymentLinkId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_PaymentLinks_AspNetUsers_UserId",
                table: "PaymentLinks");

            migrationBuilder.DropIndex(
                name: "IX_PaymentLinks_UserId",
                table: "PaymentLinks");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PaymentLinkId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BankTransfer",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Cash",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "ClientAddress",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "ClientEmail",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "ClientName",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "ExpireDate",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "MaxPaymentsNumber",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "PayPal",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "PaymentOptions_Tax",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "TermsAndConditions",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PaymentLinks");

            migrationBuilder.DropColumn(
                name: "PaymentLinkId",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "ClientPhone",
                table: "PaymentLinks",
                newName: "IsActive");

            migrationBuilder.AlterColumn<string>(
                name: "ContactInformations_WhatsApp",
                table: "Stores",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentsNumber",
                table: "PaymentLinks",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "PaymentLinks",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "PaymentLinks",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceId",
                table: "PaymentLinks",
                type: "nvarchar(8)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "PaymentLinks",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                table: "PaymentLinks",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Terms",
                table: "PaymentLinks",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "ar",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5321));

            migrationBuilder.UpdateData(
                table: "Languages",
                keyColumn: "Id",
                keyValue: "en",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5328));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ap",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5548));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "bt",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5527));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ca",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5509));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "cc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5518));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dc",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5523));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "dl",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5626));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "gp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5553));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "ma",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5558));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "pp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5532));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sa",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5620));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "sp",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5564));

            migrationBuilder.UpdateData(
                table: "PaymentMethods",
                keyColumn: "Id",
                keyValue: "st",
                column: "CreatedAt",
                value: new DateTime(2025, 9, 22, 2, 26, 24, 984, DateTimeKind.Utc).AddTicks(5544));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentLinks_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId",
                unique: true,
                filter: "[InvoiceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentLinks_Invoices_InvoiceId",
                table: "PaymentLinks",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }
    }
}
