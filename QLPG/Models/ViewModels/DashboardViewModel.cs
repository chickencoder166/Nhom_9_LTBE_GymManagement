using System.Collections.Generic;

namespace QLPG_a.Models.ViewModels
{
    public class MonthlyRevenue
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal Total { get; set; }
    }

    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalMembers { get; set; }
        public int ActiveSubscriptionsCount { get; set; }
        public int ExpiredMembersCount { get; set; }
        public List<MonthlyRevenue> Revenues { get; set; } = new();
    }

    public class TopPackageStat
    {
        public string PackageName { get; set; } = string.Empty;
        public int RegistrationCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class ReportsViewModel
    {
        public List<MonthlyRevenue> Revenues { get; set; } = new();
        public int ActiveMembers { get; set; }
        public int ExpiredMembers { get; set; }
        public List<TopPackageStat> TopPackages { get; set; } = new();
    }
}
