using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class AddClientIdToTrainingProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "TrainingProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrainingrequestTrequestId",
                table: "TrainingProgresses",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrequestId",
                table: "TrainingProgresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgresses_ClientId",
                table: "TrainingProgresses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgresses_TrainingrequestTrequestId",
                table: "TrainingProgresses",
                column: "TrainingrequestTrequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingProgresses_client_ClientId",
                table: "TrainingProgresses",
                column: "ClientId",
                principalTable: "client",
                principalColumn: "ClientID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrainingrequestTrequestId",
                table: "TrainingProgresses",
                column: "TrainingrequestTrequestId",
                principalTable: "trainingrequest",
                principalColumn: "TRequestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingProgresses_client_ClientId",
                table: "TrainingProgresses");

            migrationBuilder.DropForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_TrainingProgresses_ClientId",
                table: "TrainingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_TrainingProgresses_TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "TrainingProgresses");

            migrationBuilder.DropColumn(
                name: "TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropColumn(
                name: "TrequestId",
                table: "TrainingProgresses");
        }
    }
}
