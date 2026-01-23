using System;

namespace QLPG_a.Models
{
    // Keep only gym-related placeholders needed by the app
    public class Member
    {
        public int Id { get; set; }
        public string? MembershipNumber { get; set; }
        public string? FullName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MembershipPlan { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? JoinDate { get; set; }
    }

    // Other domain models are defined in their own files (User, Subcription, DangKyGoi, ThongBao, GoiTap)
}
