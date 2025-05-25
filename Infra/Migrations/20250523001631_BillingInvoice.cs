using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class BillingInvoice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.AddColumn<int>(
                name: "BillingInvoiceId",
                table: "ServiceOrders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BillingInvoice",
                columns: table => new
                {
                    BillingInvoiceId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillingInvoice", x => x.BillingInvoiceId);
                    table.ForeignKey(
                        name: "FK_BillingInvoice_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceOrders_BillingInvoiceId",
                table: "ServiceOrders",
                column: "BillingInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_BillingInvoice_ClientId",
                table: "BillingInvoice",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_BillingInvoice_BillingInvoiceId",
                table: "ServiceOrders",
                column: "BillingInvoiceId",
                principalTable: "BillingInvoice",
                principalColumn: "BillingInvoiceId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId",
                principalTable: "OrderPayments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_BillingInvoice_BillingInvoiceId",
                table: "ServiceOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders");

            migrationBuilder.DropTable(
                name: "BillingInvoice");

            migrationBuilder.DropIndex(
                name: "IX_ServiceOrders_BillingInvoiceId",
                table: "ServiceOrders");

            migrationBuilder.DropColumn(
                name: "BillingInvoiceId",
                table: "ServiceOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceOrders_OrderPayments_PerOrderPaymentId",
                table: "ServiceOrders",
                column: "PerOrderPaymentId",
                principalTable: "OrderPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
