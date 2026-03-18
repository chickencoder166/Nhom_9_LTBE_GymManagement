using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Tên gói")]
        public string TenGoi { get; set; } = string.Empty;

        [Required]
        [Range(1, 24, ErrorMessage = "Thời hạn phải lớn hơn 0 tháng")]
        [Display(Name = "Thời hạn (tháng)")]
        public int ThoiHan { get; set; }

        [Range(typeof(decimal), "0.01", "999999999", ErrorMessage = "Giá gói phải lớn hơn 0")]
        [Display(Name = "Giá")]
        public decimal Gia { get; set; }

        [StringLength(1000)]
        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        public ICollection<DangKiGoi> DangKiGois { get; set; } = new List<DangKiGoi>();
    }
}
