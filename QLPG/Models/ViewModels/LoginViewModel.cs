using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LabToChucWebsite.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Tài khoản")]
        public string UserName { get; set; } = string.Empty;

        [DisplayName("Mật khẩu")]
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}