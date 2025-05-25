using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Migrations
{
    /// <inheritdoc />
    public partial class ClientPatientOwnRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ClientId",
                table: "Patients");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Patients",
                newName: "PatientInternalId");

            migrationBuilder.AddColumn<string>(
                name: "PatientName",
                table: "ServiceOrders",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                columns: new[] { "ClientId", "PatientInternalId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientName",
                table: "ServiceOrders");

            migrationBuilder.RenameColumn(
                name: "PatientInternalId",
                table: "Patients",
                newName: "PatientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ClientId",
                table: "Patients",
                column: "ClientId");
        }
    }
}
