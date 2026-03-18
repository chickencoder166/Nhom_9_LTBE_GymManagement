using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    /// <summary>
    /// ViewModel for GoiTap (Package) management - API and form binding
    /// </summary>
    public class GoiTapViewModel
    {
        [Display(Name = "Mã gói")]
        public string? Id { get; set; }

        [Required(ErrorMessage = "Mã gói là bắt buộc")]
        [Display(Name = "Mã gói")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Mã gói từ 3-20 ký tự")]
        public required string MaGoiTap { get; set; }

        [Required(ErrorMessage = "Tên gói là bắt buộc")]
        [Display(Name = "Tên gói")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên gói từ 3-100 ký tự")]
        public required string TenGoi { get; set; }

        [Required(ErrorMessage = "Thời hạn là bắt buộc")]
        [Display(Name = "Thời hạn (tháng)")]
        [Range(1, 60, ErrorMessage = "Thời hạn từ 1-60 tháng")]
        public int ThoiHan { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Display(Name = "Giá (VNĐ)")]
        [Range(0.01, 999999999, ErrorMessage = "Giá phải lớn hơn 0")]
        [DataType(DataType.Currency)]
        public decimal Gia { get; set; }

        [Display(Name = "Mô tả")]
        [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự")]
        public string? MoTa { get; set; }

        /// <summary>
        /// Display formatted version of Gia for UI
        /// </summary>
        [Display(Name = "Giá")]
        public string? GiaFormatted => Gia.ToString("C0", new System.Globalization.CultureInfo("vi-VN"));
    }
}
