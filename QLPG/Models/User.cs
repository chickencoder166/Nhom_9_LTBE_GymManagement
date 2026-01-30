using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLPG_a.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã người dùng")]
        [StringLength(10, ErrorMessage = "Mã người dùng tối đa 10 ký tự")]
        [Display(Name = "Mã Người Dùng")]
        public string MembershipNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = string.Empty;

        // Stored password hash
        [Required]
        [Display(Name = "Mật khẩu (hash)")]
        public string PasswordHash { get; set; } = string.Empty;

        // Plain password for input only (not mapped to DB)
        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6 đến 100 ký tự")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100)]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [Display(Name = "Ngày Sinh")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [StringLength(20)]
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Vai trò")]
        public string Role { get; set; } = "Member"; // Admin or Member
    }
}
