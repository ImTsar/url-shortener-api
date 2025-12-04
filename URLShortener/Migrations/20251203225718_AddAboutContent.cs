using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URLShortener.Migrations
{
    /// <inheritdoc />
    public partial class AddAboutContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AboutContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AboutContents", x => x.Id);
                });
            migrationBuilder.Sql(
                "INSERT INTO AboutContents (Content) VALUES ('This URL Shortener generates a short code by randomly selecting characters from a set of letters and digits. The generator creates a fixed-length string, ensuring that each position is filled with a randomly chosen character. This approach produces simple, fast, and collision-resistant short codes suitable for redirecting long URLs.');"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AboutContents");
        }
    }
}
