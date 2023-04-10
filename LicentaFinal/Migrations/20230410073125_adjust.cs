using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicWeb.Migrations
{
    /// <inheritdoc />
    public partial class adjust : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EmailStudent",
                table: "Adeverinte",
                newName: "IdStudent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdStudent",
                table: "Adeverinte",
                newName: "EmailStudent");
        }
    }
}
