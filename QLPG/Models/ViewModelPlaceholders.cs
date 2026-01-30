using System;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class FormContact
    {
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Chủ đề")]
        public string ChuDe { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; } = string.Empty;
    }

    // Add legacy-named FormLienHe used by views (keeps compatibility)
    public class FormLienHe
    {
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Mã Sinh Viên")]
        public string MaSinhVien { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Chủ đề")]
        public string ChuDe { get; set; } = string.Empty;

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; } = string.Empty;
    }
}
