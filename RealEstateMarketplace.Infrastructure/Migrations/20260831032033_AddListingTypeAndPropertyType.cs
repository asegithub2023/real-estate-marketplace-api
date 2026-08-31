using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RealEstateMarketplace.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddListingTypeAndPropertyType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ListingType",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PropertyType",
                table: "Properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ListingType",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Properties");
        }
    }
}
