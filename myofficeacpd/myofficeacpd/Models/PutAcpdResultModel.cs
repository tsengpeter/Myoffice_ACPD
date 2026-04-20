namespace myofficeacpd.Models
{
    public class PutAcpdResultModel
    {
        public string Sid { get; set; } = null!;
        public string? Cname { get; set; }
        public string? Ename { get; set; }
        public string? Sname { get; set; }
        public string? Email { get; set; }
        public byte? Status { get; set; }
        public bool? Stop { get; set; }
        public string? StopMemo { get; set; }
        public string? LoginId { get; set; }
        public string? Memo { get; set; }
    }
}
