using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPG_a.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diems");

            migrationBuilder.RenameColumn(
                name: "DiemSo",
                table: "DiemHocPhans",
                newName: "DiemTrungBinh");

            migrationBuilder.AddColumn<int>(
                name: "SoTinChi",
                table: "HocPhans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "DiemChuyenCan",
                table: "DiemHocPhans",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DiemThi",
                table: "DiemHocPhans",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "XepLoai",
                table: "DiemHocPhans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HocPhans_MaHocPhan",
                table: "HocPhans",
                column: "MaHocPhan",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HocPhans_MaHocPhan",
                table: "HocPhans");

            migrationBuilder.DropColumn(
                name: "SoTinChi",
                table: "HocPhans");

            migrationBuilder.DropColumn(
                name: "DiemChuyenCan",
                table: "DiemHocPhans");

            migrationBuilder.DropColumn(
                name: "DiemThi",
                table: "DiemHocPhans");

            migrationBuilder.DropColumn(
                name: "XepLoai",
                table: "DiemHocPhans");

            migrationBuilder.RenameColumn(
                name: "DiemTrungBinh",
                table: "DiemHocPhans",
                newName: "DiemSo");

            migrationBuilder.CreateTable(
                name: "Diems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiemChuyenCan = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    DiemThi = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    DiemTrungBinh = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    MaMonHoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaSinhVien = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SoTinChi = table.Column<int>(type: "int", nullable: false),
                    TenMonHoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diems", x => x.Id);
                });
        }
    }
}
