using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicWeb.Migrations
{
    /// <inheritdoc />
    public partial class updatedoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Adeverinte",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Adeverinte");
        }
    }
}
