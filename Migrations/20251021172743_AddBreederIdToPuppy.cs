using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class AddBreederIdToPuppy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BreederID",
                table: "puppy",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_puppy_BreederID",
                table: "puppy",
                column: "BreederID");

            migrationBuilder.AddForeignKey(
                name: "FK_puppy_breeder_BreederID",
                table: "puppy",
                column: "BreederID",
                principalTable: "breeder",
                principalColumn: "BreederID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_puppy_breeder_BreederID",
                table: "puppy");

            migrationBuilder.DropIndex(
                name: "IX_puppy_BreederID",
                table: "puppy");

            migrationBuilder.DropColumn(
                name: "BreederID",
                table: "puppy");
        }
    }
}
