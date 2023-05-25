using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webserver.Migrations
{
    /// <inheritdoc />
    public partial class minimumAge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "minimumAge",
                table: "Movie",
                newName: "MinimumAge");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinimumAge",
                table: "Movie",
                newName: "minimumAge");
        }
    }
}
