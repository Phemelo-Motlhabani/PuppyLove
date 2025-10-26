using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingRequestRelationToProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_TrainingProgresses_TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropColumn(
                name: "TrainingrequestTrequestId",
                table: "TrainingProgresses");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgresses_TrequestId",
                table: "TrainingProgresses",
                column: "TrequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrequestId",
                table: "TrainingProgresses",
                column: "TrequestId",
                principalTable: "trainingrequest",
                principalColumn: "TRequestID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrequestId",
                table: "TrainingProgresses");

            migrationBuilder.DropIndex(
                name: "IX_TrainingProgresses_TrequestId",
                table: "TrainingProgresses");

            migrationBuilder.AddColumn<int>(
                name: "TrainingrequestTrequestId",
                table: "TrainingProgresses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainingProgresses_TrainingrequestTrequestId",
                table: "TrainingProgresses",
                column: "TrainingrequestTrequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrainingProgresses_trainingrequest_TrainingrequestTrequestId",
                table: "TrainingProgresses",
                column: "TrainingrequestTrequestId",
                principalTable: "trainingrequest",
                principalColumn: "TRequestID");
        }
    }
}
