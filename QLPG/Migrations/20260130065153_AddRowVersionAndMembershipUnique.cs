using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPG_a.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersionAndMembershipUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "DangKyGois",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_MembershipNumber",
                table: "Users",
                column: "MembershipNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_MembershipNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "DangKyGois");
        }
    }
}
