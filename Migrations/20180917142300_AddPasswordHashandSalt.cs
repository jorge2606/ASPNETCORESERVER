using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace server.Migrations
{
    public partial class AddPasswordHashandSalt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "usuario",
                table: "AllUsers",
                newName: "Usuario");

            migrationBuilder.RenameColumn(
                name: "token",
                table: "AllUsers",
                newName: "Token");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "AllUsers",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "dni",
                table: "AllUsers",
                newName: "Dni");

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "AllUsers",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "AllUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "AllUsers");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "AllUsers");

            migrationBuilder.RenameColumn(
                name: "Usuario",
                table: "AllUsers",
                newName: "usuario");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "AllUsers",
                newName: "token");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "AllUsers",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "Dni",
                table: "AllUsers",
                newName: "dni");
        }
    }
}
