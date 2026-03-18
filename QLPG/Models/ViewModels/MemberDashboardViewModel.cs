namespace QLPG_a.Models.ViewModels
{
    public class MemberDashboardViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? AvatarPath { get; set; }
        public string MembershipNumber { get; set; } = string.Empty;

        // Current active subscription (null if none)
        public DangKiGoi? ActiveSubscription { get; set; }

        // Full registration history
        public List<DangKiGoi> History { get; set; } = new();

        // All available packages (for browsing)
        public List<GoiTap> AvailablePackages { get; set; } = new();

        public int RemainingDays => ActiveSubscription == null
            ? 0
            : (int)Math.Ceiling((ActiveSubscription.NgayKetThuc.Date - DateTime.Today).TotalDays);

        public bool IsExpiringSoon => RemainingDays > 0 && RemainingDays <= 7;
        public bool IsExpired => ActiveSubscription != null && ActiveSubscription.NgayKetThuc.Date <= DateTime.Today;
    }
}
