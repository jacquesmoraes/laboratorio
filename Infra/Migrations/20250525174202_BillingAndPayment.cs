using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class BillingAndPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAt",
                table: "BillingInvoice");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "BillingInvoice",
                newName: "TotalServiceOrdersAmount");

            migrationBuilder.RenameColumn(
                name: "IssuedAt",
                table: "BillingInvoice",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<int>(
                name: "BillingInvoiceId",
                table: "ClientPayments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "BillingInvoice",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceNumber",
                table: "BillingInvoice",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousBalance",
                table: "BillingInvoice",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_ClientPayments_BillingInvoiceId",
                table: "ClientPayments",
                column: "BillingInvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments",
                column: "BillingInvoiceId",
                principalTable: "BillingInvoice",
                principalColumn: "BillingInvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments");

            migrationBuilder.DropIndex(
                name: "IX_ClientPayments_BillingInvoiceId",
                table: "ClientPayments");

            migrationBuilder.DropColumn(
                name: "BillingInvoiceId",
                table: "ClientPayments");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "BillingInvoice");

            migrationBuilder.DropColumn(
                name: "InvoiceNumber",
                table: "BillingInvoice");

            migrationBuilder.DropColumn(
                name: "PreviousBalance",
                table: "BillingInvoice");

            migrationBuilder.RenameColumn(
                name: "TotalServiceOrdersAmount",
                table: "BillingInvoice",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "BillingInvoice",
                newName: "IssuedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidAt",
                table: "BillingInvoice",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
