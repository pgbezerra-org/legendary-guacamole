using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webserver.Migrations
{
    /// <inheritdoc />
    public partial class realestateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_Company_CompanyId1",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_CompanyId1",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "CompanyId1",
                table: "RealEstates");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "RealEstates",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_CompanyId",
                table: "RealEstates",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_Company_CompanyId",
                table: "RealEstates",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RealEstates_Company_CompanyId",
                table: "RealEstates");

            migrationBuilder.DropIndex(
                name: "IX_RealEstates_CompanyId",
                table: "RealEstates");

            migrationBuilder.AlterColumn<int>(
                name: "CompanyId",
                table: "RealEstates",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId1",
                table: "RealEstates",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RealEstates_CompanyId1",
                table: "RealEstates",
                column: "CompanyId1");

            migrationBuilder.AddForeignKey(
                name: "FK_RealEstates_Company_CompanyId1",
                table: "RealEstates",
                column: "CompanyId1",
                principalTable: "Company",
                principalColumn: "Id");
        }
    }
}
