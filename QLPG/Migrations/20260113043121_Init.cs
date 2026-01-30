using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPG_a.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSinhVien = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    MaMonHoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenMonHoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoTinChi = table.Column<int>(type: "int", nullable: false),
                    DiemChuyenCan = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    DiemThi = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    DiemTrungBinh = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    XepLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HocPhans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHocPhan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenHocPhan = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HocPhans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LopHocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLop = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenLop = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Khoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SiSo = table.Column<int>(type: "int", nullable: false),
                    GiaoVienChuNhiem = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LopHocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonHocs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaMonHoc = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TenMonHoc = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SoTinChi = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonHocs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SinhViens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSinhVien = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HoVaTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MaLop = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SinhViens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThongBaos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TieuDe = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    NgayDang = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBaos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiemHocPhans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SinhVienId = table.Column<int>(type: "int", nullable: false),
                    HocPhanId = table.Column<int>(type: "int", nullable: false),
                    DiemSo = table.Column<decimal>(type: "decimal(4,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiemHocPhans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiemHocPhans_HocPhans_HocPhanId",
                        column: x => x.HocPhanId,
                        principalTable: "HocPhans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiemHocPhans_SinhViens_SinhVienId",
                        column: x => x.SinhVienId,
                        principalTable: "SinhViens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiemHocPhans_HocPhanId",
                table: "DiemHocPhans",
                column: "HocPhanId");

            migrationBuilder.CreateIndex(
                name: "IX_DiemHocPhans_SinhVienId",
                table: "DiemHocPhans",
                column: "SinhVienId");

            migrationBuilder.CreateIndex(
                name: "IX_LopHocs_MaLop",
                table: "LopHocs",
                column: "MaLop",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonHocs_MaMonHoc",
                table: "MonHocs",
                column: "MaMonHoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SinhViens_MaSinhVien",
                table: "SinhViens",
                column: "MaSinhVien",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBaos_LoaiThongBao",
                table: "ThongBaos",
                column: "LoaiThongBao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiemHocPhans");

            migrationBuilder.DropTable(
                name: "Diems");

            migrationBuilder.DropTable(
                name: "LopHocs");

            migrationBuilder.DropTable(
                name: "MonHocs");

            migrationBuilder.DropTable(
                name: "ThongBaos");

            migrationBuilder.DropTable(
                name: "HocPhans");

            migrationBuilder.DropTable(
                name: "SinhViens");
        }
    }
}
