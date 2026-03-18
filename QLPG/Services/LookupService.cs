using Microsoft.EntityFrameworkCore;
using QLPG_a.Data;
using QLPG_a.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLPG_a.Services
{
    public class LookupService : ILookupService
    {
        private readonly IApplicationDbContext _context;

        public LookupService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Member>> GetMembersAsync()
        {
            return await _context.Members.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<GoiTap>> GetGoiTapsAsync()
        {
            return await _context.GoiTaps.AsNoTracking().ToListAsync();
        }
    }
}
