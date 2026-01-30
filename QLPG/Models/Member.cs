using System.ComponentModel.DataAnnotations;

namespace QLPG_a.Models
{
    public class Member
    {
        [Key]
        public int MemberId { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime RegisterDate { get; set; } = DateTime.Now;

        public string Package { get; set; } = string.Empty;
    }
}
