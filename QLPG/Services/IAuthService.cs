using QLPG_a.Models;
using QLPG_a.Models.ViewModels;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<User>> RegisterAsync(RegisterViewModel model);
        Task<ServiceResult<User>> ValidateLoginAsync(LoginViewModel model);
    }
}
