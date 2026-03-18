namespace QLPG_a.Models.ViewModels
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string MembershipNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? CurrentPackageName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Chưa đăng ký";
        public int? RemainingDays { get; set; }
    }
}
