using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class AddDogNameToTrainingrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_review_trainer_TrainerID",
                table: "review");

            migrationBuilder.AddColumn<string>(
                name: "DogBreed",
                table: "trainingrequest",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DogName",
                table: "trainingrequest",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerID",
                table: "review",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_review_trainer_TrainerID",
                table: "review",
                column: "TrainerID",
                principalTable: "trainer",
                principalColumn: "TrainerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_review_trainer_TrainerID",
                table: "review");

            migrationBuilder.DropColumn(
                name: "DogBreed",
                table: "trainingrequest");

            migrationBuilder.DropColumn(
                name: "DogName",
                table: "trainingrequest");

            migrationBuilder.AlterColumn<int>(
                name: "TrainerID",
                table: "review",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_review_trainer_TrainerID",
                table: "review",
                column: "TrainerID",
                principalTable: "trainer",
                principalColumn: "TrainerID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
