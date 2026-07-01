using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDA_PCCV.Migrations
{
    /// <inheritdoc />
    public partial class AddGitHubUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GitHubUsername",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GitHubUsername",
                table: "Users");
        }
    }
}
