using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class CurrentBalanceUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Credit",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "Debt",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "PreviousBalance",
                table: "BillingInvoice",
                newName: "PreviousDebit");

            migrationBuilder.AddColumn<decimal>(
                name: "PreviousCredit",
                table: "BillingInvoice",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousCredit",
                table: "BillingInvoice");

            migrationBuilder.RenameColumn(
                name: "PreviousDebit",
                table: "BillingInvoice",
                newName: "PreviousBalance");

            migrationBuilder.AddColumn<decimal>(
                name: "Credit",
                table: "Clients",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Debt",
                table: "Clients",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
