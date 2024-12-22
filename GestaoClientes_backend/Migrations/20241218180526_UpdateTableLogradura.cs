using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoClientes_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableLogradura : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logradouro_Clientes_ClienteId",
                table: "Logradouro");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro");

            migrationBuilder.RenameTable(
                name: "Logradouro",
                newName: "Logradouros");

            migrationBuilder.RenameIndex(
                name: "IX_Logradouro_ClienteId",
                table: "Logradouros",
                newName: "IX_Logradouros_ClienteId");

            migrationBuilder.AddColumn<Guid>(
                name: "IdCliente",
                table: "Logradouros",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logradouros",
                table: "Logradouros",
                column: "IdLogradouro");

            migrationBuilder.AddForeignKey(
                name: "FK_Logradouros_Clientes_ClienteId",
                table: "Logradouros",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logradouros_Clientes_ClienteId",
                table: "Logradouros");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Logradouros",
                table: "Logradouros");

            migrationBuilder.DropColumn(
                name: "IdCliente",
                table: "Logradouros");

            migrationBuilder.RenameTable(
                name: "Logradouros",
                newName: "Logradouro");

            migrationBuilder.RenameIndex(
                name: "IX_Logradouros_ClienteId",
                table: "Logradouro",
                newName: "IX_Logradouro_ClienteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Logradouro",
                table: "Logradouro",
                column: "IdLogradouro");

            migrationBuilder.AddForeignKey(
                name: "FK_Logradouro_Clientes_ClienteId",
                table: "Logradouro",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
