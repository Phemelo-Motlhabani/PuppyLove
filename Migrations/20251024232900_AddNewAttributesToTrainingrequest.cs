using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class AddNewAttributesToTrainingrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "trainingrequest",
                keyColumn: "TrainingProgram",
                keyValue: null,
                column: "TrainingProgram",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "TrainingProgram",
                table: "trainingrequest",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "trainingrequest",
                keyColumn: "DogName",
                keyValue: null,
                column: "DogName",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "DogName",
                table: "trainingrequest",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "trainingrequest",
                keyColumn: "DogBreed",
                keyValue: null,
                column: "DogBreed",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "DogBreed",
                table: "trainingrequest",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalInfo",
                table: "trainingrequest",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalNotes",
                table: "trainingrequest",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "DogAge",
                table: "trainingrequest",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalInfo",
                table: "trainingrequest");

            migrationBuilder.DropColumn(
                name: "AdditionalNotes",
                table: "trainingrequest");

            migrationBuilder.DropColumn(
                name: "DogAge",
                table: "trainingrequest");

            migrationBuilder.AlterColumn<string>(
                name: "TrainingProgram",
                table: "trainingrequest",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DogName",
                table: "trainingrequest",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DogBreed",
                table: "trainingrequest",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
