using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    /// <summary>
    /// Form contact model - lựa chọn duy nhất để tránh duplicate
    /// </summary>
    public class FormContact
    {
        [Display(Name = "Họ và tên")]
        public string? HoTen { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string? Email { get; set; }

        [Display(Name = "Chủ đề")]
        public string? ChuDe { get; set; }

        [Display(Name = "Nội dung")]
        public string? NoiDung { get; set; }
    }

    /// <summary>
    /// FormLienHe - keepslegacy "Mã Sinh Viên" field for backward compatibility with existing view
    /// </summary>
    public class FormLienHe : FormContact
    {
        [Display(Name = "Mã Sinh Viên")]
        public string? MaSinhVien { get; set; }
    }
}

