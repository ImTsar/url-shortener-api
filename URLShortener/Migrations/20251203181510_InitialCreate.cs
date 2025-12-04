using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace URLShortener.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.Sql(@"
                INSERT INTO Users (Username, PasswordHash, Role)
                VALUES 
                (
                    'admin',
                    'AQAAAAIAAYagAAAAEC06kpml/wK3CmGNAdxtPs+9fS9GRBgBd9yC+a/dMz2XjUN46ODJ1oTpkIRpVTGrdQ==',
                    1
                ),
                (
                    'user',
                    'AQAAAAIAAYagAAAAEFduKeHe27UwzFH1PL6DGDDH45jsPX64w0gRhKLaETXI/m0kNCWkFtSRrFXKlTVIdA==',
                    0
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
