using System;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }
        public string FullName { get; set; }
    }

    public class FormContact
    {
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Chủ đề")]
        public string ChuDe { get; set; }

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
    }

    // Add legacy-named FormLienHe used by views (keeps compatibility)
    public class FormLienHe
    {
        [Display(Name = "Họ và tên")]
        public string HoTen { get; set; }

        [Display(Name = "Mã Sinh Viên")]
        public string MaSinhVien { get; set; }

        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Chủ đề")]
        public string ChuDe { get; set; }

        [Display(Name = "Nội dung")]
        public string NoiDung { get; set; }
    }
}
