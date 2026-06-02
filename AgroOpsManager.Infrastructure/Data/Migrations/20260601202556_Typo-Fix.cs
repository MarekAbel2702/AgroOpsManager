using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgroOpsManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class TypoFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AreInHectares",
                table: "Fields",
                newName: "AreaInHectares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AreaInHectares",
                table: "Fields",
                newName: "AreInHectares");
        }
    }
}
