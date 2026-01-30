using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPG_a.Models
{
    public class GoiTap
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã gói")]
        public string MaGoiTap { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Tên gói tập")]
        public string TenGoi { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Thời hạn (tháng)")]
        public int ThoiHan { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Giá gói (VNĐ)")]
        public decimal Gia { get; set; }

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string MoTa { get; set; } = string.Empty;

        // Navigation: danh sách đăng ký gói sử dụng gói này
        public ICollection<DangKyGoi>? DangKyGois { get; set; }
    }
}
