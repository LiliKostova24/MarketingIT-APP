using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingIT.Migrations
{
    public partial class MakeImageUrlActuallyNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
           name: "ImageUrl",
            table: "Posts",
             type: "nvarchar(max)",
            nullable: true,              // allow NULLs
             oldClrType: typeof(string),
            oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
