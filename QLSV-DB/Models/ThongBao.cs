using System;
using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models
{
    public class ThongBao
    {
        public int Id { get; set; }

        [Display(Name = "Tiêu Đề")]
        public string TieuDe { get; set; }

        [Display(Name = "Nội Dung")]
        public string NoiDung { get; set; }

        [Display(Name = "Ngày Đăng")]
        public DateTime NgayDang { get; set; }

        [Display(Name = "Loại Thông Báo")]
        public string LoaiThongBao { get; set; } // "quantrong", "hocvu", "chung"

        public string LayMauBadge()
        {
            return LoaiThongBao switch
            {
                "quantrong" => "danger",
                "hocvu" => "primary",
                "chung" => "secondary",
                _ => "secondary"
            };
        }

        public string LayTenLoai()
        {
            return LoaiThongBao switch
            {
                "quantrong" => "Quan Trọng",
                "hocvu" => "Học Vụ",
                "chung" => "Chung",
                _ => "Chung"
            };
        }
    }
}
