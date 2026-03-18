using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("Mã người dùng")]
        [StringLength(10, ErrorMessage = "Mã người dùng tối đa 10 ký tự")]
        public required string MembershipNumber { get; set; } = string.Empty;

        [Required]
        [DisplayName("Tài khoản")]
        public required string UserName { get; set; } = string.Empty;

        [DisplayName("Mật khẩu")]
        [Required, DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu từ 8 đến 100 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm 1 chữ hoa và 1 chữ số")]
        public required string Password { get; set; } = string.Empty;

        [Required]
        [DisplayName("Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public required string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [DisplayName("Họ và Tên")]
        public required string FullName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [DisplayName("Giới tính")]
        public string? Gender { get; set; }

        [DisplayName("Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [Required]
        [DisplayName("Số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public required string Phone { get; set; } = string.Empty;
    }
}
