using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiEstudio.Migrations
{
    public partial class sqlitelocal_migration_707 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Concept",
                table: "MovementModel",
                type: "TEXT",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Concept",
                table: "MovementModel");
        }
    }
}
