using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiEstudio.Migrations
{
    public partial class sqlitelocal_migration_988 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClientType",
                table: "ClientModel",
                newName: "Type");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "MovementModel",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "MovementModel");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "ClientModel",
                newName: "ClientType");
        }
    }
}