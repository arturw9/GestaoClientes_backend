using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoClientes_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddLogradouroTableCliente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logradouros_Clientes_ClienteId",
                table: "Logradouros");

            migrationBuilder.DropIndex(
                name: "IX_Logradouros_ClienteId",
                table: "Logradouros");

            migrationBuilder.DropColumn(
                name: "ClienteId",
                table: "Logradouros");

            migrationBuilder.CreateIndex(
                name: "IX_Logradouros_IdCliente",
                table: "Logradouros",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Logradouros_Clientes_IdCliente",
                table: "Logradouros",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logradouros_Clientes_IdCliente",
                table: "Logradouros");

            migrationBuilder.DropIndex(
                name: "IX_Logradouros_IdCliente",
                table: "Logradouros");

            migrationBuilder.AddColumn<Guid>(
                name: "ClienteId",
                table: "Logradouros",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logradouros_ClienteId",
                table: "Logradouros",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logradouros_Clientes_ClienteId",
                table: "Logradouros",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id");
        }
    }
}
