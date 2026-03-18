using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models
{
    public class ThongBao
    {
        public int Id { get; set; }

        [Required]
        [StringLength(500)]
        public string TieuDe { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? NoiDung { get; set; }

        [StringLength(50)]
        public string? LoaiThongBao { get; set; }

        public DateTime NgayDang { get; set; }

        public string LayMauBadge()
        {
            return LoaiThongBao?.ToLowerInvariant() switch
            {
                "quantrong" => "danger",
                "hocvu" => "primary",
                _ => "secondary"
            };
        }

        public string LayTenLoai()
        {
            return LoaiThongBao?.ToLowerInvariant() switch
            {
                "quantrong" => "Quan trọng",
                "hocvu" => "Học vụ",
                _ => "Thông báo"
            };
        }
    }
}
