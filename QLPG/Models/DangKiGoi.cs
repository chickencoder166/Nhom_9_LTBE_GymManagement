using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPG_a.Models
{
    public class DangKiGoi
    {
        public DangKiGoi()
        {
            NgayBatDau = DateTime.Now;
            TrangThai = "Đang hoạt động";
        }

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã đăng ký")]
        public string MaDangKy { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn hội viên")]
        [Display(Name = "Hội viên")]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }

        // FK to GoiTap (gói tập)
        [Required(ErrorMessage = "Vui lòng chọn gói tập")]
        [Display(Name = "Gói tập")]
        public int GoiTapId { get; set; }

        [ForeignKey(nameof(GoiTapId))]
        public GoiTap? GoiTap { get; set; }

        [Display(Name = "Ngày bắt đầu")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày kết thúc")]
        public DateTime NgayKetThuc { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Tổng tiền")]
        public decimal TongTien { get; set; }

        [StringLength(50)]
        [Display(Name = "Trạng thái")]
        public string? TrangThai { get; set; } // "Đang hoạt động" or "Hết hạn"

        [StringLength(500)]
        [Display(Name = "Ghi chú")]
        public string? GhiChu { get; set; }

        public bool IsActive()
        {
            if (string.Equals(TrangThai, "Hết hạn", StringComparison.OrdinalIgnoreCase)) return false;
            return NgayKetThuc > DateTime.Now;
        }

        // Theo đề bài: Ngày kết thúc = Ngày bắt đầu + (Thời hạn * 30 ngày)
        public void CapNhatNgayKetThucBoiGoi()
        {
            if (GoiTap != null)
            {
                NgayKetThuc = NgayBatDau.AddDays(GoiTap.ThoiHan * 30);
            }
        }

        public void CapNhatTrangThai()
        {
            TrangThai = NgayKetThuc > DateTime.Now ? "Đang hoạt động" : "Hết hạn";
        }

        // Optimistic concurrency token (row version)
        [Timestamp]
        public byte[]? RowVersion { get; set; }
    }
}
