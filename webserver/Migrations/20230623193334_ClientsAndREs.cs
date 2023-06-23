using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webserver.Migrations
{
    /// <inheritdoc />
    public partial class ClientsAndREs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstate_AspNetUsers_CompanyId1",
                table: "RealEstate");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstate",
                table: "RealEstate");

            migrationBuilder.RenameTable(
                name: "RealEstate",
                newName: "RealEstates");

            migrationBuilder.RenameIndex(
                name: "IX_RealEstate_CompanyId1",
                table: "RealEstates",
                newName: "IX_RealEstates_CompanyId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstates",
                table: "RealEstates",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_AspNetUsers_CompanyId1",
                table: "RealEstates",
                column: "CompanyId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_AspNetUsers_CompanyId1",
                table: "RealEstates");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RealEstates",
                table: "RealEstates");

            migrationBuilder.RenameTable(
                name: "RealEstates",
                newName: "RealEstate");

            migrationBuilder.RenameIndex(
                name: "IX_RealEstates_CompanyId1",
                table: "RealEstate",
                newName: "IX_RealEstate_CompanyId1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RealEstate",
                table: "RealEstate",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstate_AspNetUsers_CompanyId1",
                table: "RealEstate",
                column: "CompanyId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
