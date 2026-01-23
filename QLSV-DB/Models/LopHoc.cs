using System.ComponentModel.DataAnnotations;

namespace QLSV_DB.Models
{
    public class LopHoc
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã Lớp")]
        public string MaLop { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên Lớp")]
        public string TenLop { get; set; }

        [StringLength(100)]
        [Display(Name = "Khoa")]
        public string Khoa { get; set; }

        [Display(Name = "Sĩ Số")]
        public int SiSo { get; set; }

        [StringLength(100)]
        [Display(Name = "Giáo Viên Chủ Nhiệm")]
        public string GiaoVienChuNhiem { get; set; }
    }
}
