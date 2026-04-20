using System.ComponentModel.DataAnnotations;

namespace myofficeacpd.Models
{
    public class PostAcpdRequestModel
    {
        [Required]
        [StringLength(60)]
        public string Cname { get; set; } = null!;

        [StringLength(40)]
        public string? Ename { get; set; }

        [StringLength(40)]
        public string? Sname { get; set; }

        [StringLength(60)]
        [EmailAddress]
        public string? Email { get; set; }

        public byte? Status { get; set; }

        public bool? Stop { get; set; }

        [StringLength(60)]
        public string? StopMemo { get; set; }

        [Required]
        [StringLength(30)]
        public string LoginId { get; set; } = null!;

        [Required]
        [StringLength(60)]
        public string LoginPwd { get; set; } = null!;

        [StringLength(600)]
        public string? Memo { get; set; }
    }
}
