using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoClientes_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableClientess2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Logradouro");

            migrationBuilder.AddColumn<Guid>(
                name: "IdLogradouro",
                table: "Logradouro",
                type: "uniqueidentifier",
                maxLength: 100,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro",
                column: "IdLogradouro");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro");

            migrationBuilder.DropColumn(
                name: "IdLogradouro",
                table: "Logradouro");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Logradouro",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro",
                column: "Id");
        }
    }
}
