using System.ComponentModel.DataAnnotations;

namespace QLSV_DB.Models
{
    public class HocPhan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã học phần là bắt buộc")]
        [Display(Name = "Mã học phần")]
        public string MaHocPhan { get; set; }

        [Required(ErrorMessage = "Tên học phần là bắt buộc")]
        [Display(Name = "Tên học phần")]
        public string TenHocPhan { get; set;}

        [Required(ErrorMessage = "Số tín chỉ là bắt buộc")]
        [Range(2, 4, ErrorMessage = "Số tín chỉ phải nằm trong khoảng từ 2 đến 4")]
        [Display(Name = "Số tín chỉ")]
        public int SoTinChi { get; set; }
    }
}
