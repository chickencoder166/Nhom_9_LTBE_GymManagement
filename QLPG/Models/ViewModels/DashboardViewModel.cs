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
        public List<MonthlyRevenue> Revenues { get; set; } = new List<MonthlyRevenue>();
    }

    public class ReportsViewModel
    {
        public List<MonthlyRevenue> Revenues { get; set; } = new List<MonthlyRevenue>();
        public int ActiveMembers { get; set; }
    }
}
