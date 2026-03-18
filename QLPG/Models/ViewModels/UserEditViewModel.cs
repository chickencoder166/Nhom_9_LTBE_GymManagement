using System;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    /// <summary>
    /// ViewModel for editing User accounts (Admin only)
    /// </summary>
    public class UserEditViewModel
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã người dùng là bắt buộc")]
        [Display(Name = "Mã người dùng")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Mã người dùng từ 3-10 ký tự")]
        public required string MembershipNumber { get; set; }

        [Display(Name = "Tài khoản")]
        [StringLength(50)]
        public string? UserName { get; set; }

        [Display(Name = "Họ và Tên")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ và Tên từ 2-100 ký tự")]
        public string? FullName { get; set; }

        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Giới tính")]
        [StringLength(20)]
        public string? Gender { get; set; }

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Vai trò là bắt buộc")]
        [Display(Name = "Vai trò")]
        [StringLength(20)]
        public required string Role { get; set; } = "Member";

        [Display(Name = "Đổi mật khẩu?")]
        public bool ChangePassword { get; set; } = false;

        [Display(Name = "Mật khẩu mới")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu từ 8 đến 100 ký tự")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, gồm 1 chữ hoa và 1 chữ số")]
        public string? NewPassword { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không khớp")]
        public string? ConfirmPassword { get; set; }
    }
}
