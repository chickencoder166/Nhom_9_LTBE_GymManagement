using System.ComponentModel.DataAnnotations;

namespace QLSV_DB.Models
{
    public class MonHoc
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Mã Môn Học")]
        public string MaMonHoc { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tên Môn Học")]
        public string TenMonHoc { get; set; }

        [Range(1, 10)]
        [Display(Name = "Số Tín Chỉ")]
        public int SoTinChi { get; set; }
    }
}
