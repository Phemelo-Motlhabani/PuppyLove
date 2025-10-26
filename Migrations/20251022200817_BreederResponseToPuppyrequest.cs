using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PupV1.Migrations
{
    /// <inheritdoc />
    public partial class BreederResponseToPuppyrequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BreederResponse",
                table: "puppyrequest",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "puppyrequest",
                type: "text",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "RequestDate",
                table: "puppyrequest",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "puppyrequest",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BreederResponse",
                table: "puppyrequest");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "puppyrequest");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "puppyrequest");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "puppyrequest");
        }
    }
}
