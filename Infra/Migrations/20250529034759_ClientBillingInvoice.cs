using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class ClientBillingInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments",
                column: "BillingInvoiceId",
                principalTable: "BillingInvoice",
                principalColumn: "BillingInvoiceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientPayments_BillingInvoice_BillingInvoiceId",
                table: "ClientPayments",
                column: "BillingInvoiceId",
                principalTable: "BillingInvoice",
                principalColumn: "BillingInvoiceId");
        }
    }
}
