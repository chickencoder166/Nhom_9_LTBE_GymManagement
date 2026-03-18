using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLPG_a.Migrations
{
    /// <inheritdoc />
    public partial class AlignGymSchemaNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[DangKyGois]', 'U') IS NOT NULL
    AND OBJECT_ID(N'[Legacy_DangKyGois]', 'U') IS NULL
BEGIN
    EXEC sp_rename N'[DangKyGois]', N'Legacy_DangKyGois';
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[DangKiGoiTaps]', 'U') IS NOT NULL
BEGIN
    DECLARE @fkSql nvarchar(max) = N'';
    SELECT @fkSql += N'ALTER TABLE [' + OBJECT_SCHEMA_NAME(fk.parent_object_id) + N'].['
        + OBJECT_NAME(fk.parent_object_id) + N'] DROP CONSTRAINT [' + fk.name + N'];'
    FROM sys.foreign_keys fk
    WHERE fk.referenced_object_id = OBJECT_ID(N'[DangKiGoiTaps]');

    IF LEN(@fkSql) > 0
        EXEC sp_executesql @fkSql;

    DECLARE @pkName sysname;
    SELECT @pkName = kc.name
    FROM sys.key_constraints kc
    JOIN sys.tables t ON kc.parent_object_id = t.object_id
    WHERE kc.type = 'PK' AND t.name = 'DangKiGoiTaps';

    IF @pkName IS NOT NULL
        EXEC('ALTER TABLE [DangKiGoiTaps] DROP CONSTRAINT [' + @pkName + ']');

    IF OBJECT_ID(N'[GoiTaps]', 'U') IS NULL
        EXEC sp_rename N'[DangKiGoiTaps]', N'GoiTaps';
END
");

            migrationBuilder.RenameColumn(
                name: "MembershipGoiTap",
                table: "Members",
                newName: "Package");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "ThongBaos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiThongBao",
                table: "ThongBaos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Members",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[GoiTaps]', 'U') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM sys.key_constraints kc
        JOIN sys.tables t ON kc.parent_object_id = t.object_id
        WHERE kc.type = 'PK' AND t.name = 'GoiTaps'
    )
BEGIN
    ALTER TABLE [GoiTaps] ADD CONSTRAINT [PK_GoiTaps] PRIMARY KEY ([Id]);
END
");

            migrationBuilder.CreateTable(
                name: "DangKiGois",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDangKy = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    GoiTapId = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKiGois", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DangKiGois_GoiTaps_GoiTapId",
                        column: x => x.GoiTapId,
                        principalTable: "GoiTaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DangKiGois_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKiGois_GoiTapId",
                table: "DangKiGois",
                column: "GoiTapId");

            migrationBuilder.CreateIndex(
                name: "IX_DangKiGois_MemberId",
                table: "DangKiGois",
                column: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DangKiGois");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[Legacy_DangKyGois]', 'U') IS NOT NULL
    AND OBJECT_ID(N'[DangKyGois]', 'U') IS NULL
BEGIN
    EXEC sp_rename N'[Legacy_DangKyGois]', N'DangKyGois';
END
");

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[GoiTaps]', 'U') IS NOT NULL
BEGIN
    DECLARE @pkName sysname;
    SELECT @pkName = kc.name
    FROM sys.key_constraints kc
    JOIN sys.tables t ON kc.parent_object_id = t.object_id
    WHERE kc.type = 'PK' AND t.name = 'GoiTaps';

    IF @pkName IS NOT NULL
        EXEC('ALTER TABLE [GoiTaps] DROP CONSTRAINT [' + @pkName + ']');

    IF OBJECT_ID(N'[DangKiGoiTaps]', 'U') IS NULL
        EXEC sp_rename N'[GoiTaps]', N'DangKiGoiTaps';
END
");

            migrationBuilder.RenameColumn(
                name: "Package",
                table: "Members",
                newName: "MembershipGoiTap");

            migrationBuilder.AlterColumn<string>(
                name: "NoiDung",
                table: "ThongBaos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LoaiThongBao",
                table: "ThongBaos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Members",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.Sql(@"
IF OBJECT_ID(N'[DangKiGoiTaps]', 'U') IS NOT NULL
    AND NOT EXISTS (
        SELECT 1
        FROM sys.key_constraints kc
        JOIN sys.tables t ON kc.parent_object_id = t.object_id
        WHERE kc.type = 'PK' AND t.name = 'DangKiGoiTaps'
    )
BEGIN
    ALTER TABLE [DangKiGoiTaps] ADD CONSTRAINT [PK_DangKiGoiTaps] PRIMARY KEY ([Id]);
END
");

        }
    }
}
