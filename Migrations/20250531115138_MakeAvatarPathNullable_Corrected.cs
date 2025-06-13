using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MarketingIT.Migrations
{
    public partial class MakeAvatarPathNullable_Corrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AvatarPath",
                table: "AspNetUsers",
                type: "nvarchar(200)",       // or "nvarchar(max)" depending on your previous type
                maxLength: 200,              // if you used a max length of 200 originally
                nullable: true,              // <— this is the key: allow NULL
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",    // or whatever the existing type is
                oldMaxLength: 200);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
