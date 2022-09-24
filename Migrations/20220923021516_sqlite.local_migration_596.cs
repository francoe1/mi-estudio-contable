using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiEstudio.Migrations
{
    public partial class sqlitelocal_migration_596 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClientId",
                table: "MovementModel",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "MovementModel");
        }
    }
}