using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Tài khoản")]
        public required string UserName { get; set; } = string.Empty;

        [DisplayName("Mật khẩu")]
        [Required, DataType(DataType.Password)]
        public required string Password { get; set; } = string.Empty;
    }
}
