using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LabToChucWebsite.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }

        [DisplayName("Mật khẩu")]
        [Required, DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; }
    }
}