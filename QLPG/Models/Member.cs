using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [StringLength(20)]
        [Display(Name = "Mã hội viên")]
        public string? MembershipNumber { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Họ tên")]
        public string FullName { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? DateOfBirth { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Số điện thoại phải gồm đúng 10 chữ số")]
        [Display(Name = "Số điện thoại")]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày tham gia")]
        public DateTime? JoinDate { get; set; }

        [Display(Name = "Gói hiện tại")]
        public string? Package { get; set; }
    }
}
