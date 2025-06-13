using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingIT.Migrations
{
    public partial class AddSurveyFieldsToPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SurveyOptions",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SurveyQuestion",
                table: "Posts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyOptions",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "SurveyQuestion",
                table: "Posts");
        }
    }
}
