using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QLSV_DB.Models
{
    public class DiemHocPhan
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn sinh viên")]
        [Display(Name = "Sinh viên")]
        public int SinhVienId { get; set; }

        [ForeignKey(nameof(SinhVienId))]
        public SinhVien? SinhVien { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học phần")]
        [Display(Name = "Học phần")]
        public int HocPhanId { get; set; }

        [ForeignKey(nameof(HocPhanId))]
        public HocPhan? HocPhan { get; set; }

        [Required(ErrorMessage = "Điểm chuyên cần là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm chuyên cần")]
        public float DiemChuyenCan { get; set; }

        [Required(ErrorMessage = "Điểm thi là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm thi")]
        public float DiemThi { get; set; }

        [Display(Name = "Điểm trung bình")]
        public float DiemTrungBinh { get; set; }

        [StringLength(50)]
        [Display(Name = "Xếp loại")]
        public string? XepLoai { get; set; }

        // Tính điểm trung bình và xếp loại
        public void TinhDiem()
        {
            DiemTrungBinh = DiemChuyenCan * 0.3f + DiemThi * 0.7f;
            XepLoai = LayXepLoai();
        }

        private string LayXepLoai()
        {
            if (DiemTrungBinh >= 8.5) return "Giỏi";
            if (DiemTrungBinh >= 7.0) return "Khá";
            if (DiemTrungBinh >= 5.0) return "Trung Bình";
            return "Yếu";
        }
    }

    public class Performance
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn hội viên")]
        [Display(Name = "Hội viên")]
        public int MemberId { get; set; }

        [ForeignKey(nameof(MemberId))]
        public Member? Member { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn học phần")]
        [Display(Name = "Bài tập")]
        public int HocPhanId { get; set; }

        [ForeignKey(nameof(HocPhanId))]
        public HocPhan? Workout { get; set; }

        [Required(ErrorMessage = "Điểm chuyên cần là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm chuyên cần")]
        public float DiemChuyenCan { get; set; }

        [Required(ErrorMessage = "Điểm thi là bắt buộc")]
        [Range(0, 10, ErrorMessage = "Điểm phải từ 0 đến 10")]
        [Display(Name = "Điểm thi")]
        public float DiemThi { get; set; }

        [Display(Name = "Điểm trung bình")]
        public float DiemTrungBinh { get; set; }

        [StringLength(50)]
        [Display(Name = "Xếp loại")]
        public string? XepLoai { get; set; }

        // Tính điểm trung bình và xếp loại
        public void TinhDiem()
        {
            DiemTrungBinh = DiemChuyenCan * 0.3f + DiemThi * 0.7f;
            XepLoai = LayXepLoai();
        }

        private string LayXepLoai()
        {
            if (DiemTrungBinh >= 8.5) return "Giỏi";
            if (DiemTrungBinh >= 7.0) return "Khá";
            if (DiemTrungBinh >= 5.0) return "Trung Bình";
            return "Yếu";
        }
    }
}
