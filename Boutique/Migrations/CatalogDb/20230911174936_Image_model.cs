using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Boutique.Migrations.CatalogDb
{
    /// <inheritdoc />
    public partial class Image_model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "ImageStored",
                table: "Image",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageStored",
                table: "Image");
        }
    }
}
