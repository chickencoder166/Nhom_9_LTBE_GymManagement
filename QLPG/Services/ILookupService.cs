using QLPG_a.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public interface ILookupService
    {
        Task<IReadOnlyList<Member>> GetMembersAsync();
        Task<IReadOnlyList<GoiTap>> GetGoiTapsAsync();
    }
}
