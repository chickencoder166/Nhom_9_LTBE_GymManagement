using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPG_a.Models
{
    public class DangKyGoi
    {
        public DangKyGoi()
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
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn gói tập")]
        [Display(Name = "Mã gói")]
        public int SubcriptionId { get; set; }

        [ForeignKey(nameof(SubcriptionId))]
        public GoiTap? Subcription { get; set; }

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

        public void CapNhatNgayKetThucBoiGoi()
        {
            if (Subcription != null)
            {
                // Use 30 days per month as business rule
                NgayKetThuc = NgayBatDau.AddDays(Subcription.ThoiHan * 30);
            }
        }

        public void CapNhatTrangThai()
        {
            TrangThai = NgayKetThuc > DateTime.Now ? "Đang hoạt động" : "Hết hạn";
        }
    }
}
