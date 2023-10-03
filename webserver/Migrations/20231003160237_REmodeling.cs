using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webserver.Migrations
{
    /// <inheritdoc />
    public partial class REmodeling : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "area",
                table: "RealEstates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "numBedrooms",
                table: "RealEstates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "percentage",
                table: "RealEstates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "rentable",
                table: "RealEstates",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "salary",
                table: "BZEmployees",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "area",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "numBedrooms",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "percentage",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "rentable",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "salary",
                table: "BZEmployees");
        }
    }
}
