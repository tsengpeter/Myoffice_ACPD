using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace myofficeacpd.Data.Entities
{
    [Table("MyOffice_ExcuteionLog")]
    public class MyOfficeExcuteionLog
    {
        [Key]
        [Column("DeLog_AutoID")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DeLogAutoId { get; set; }

        [Required]
        [Column("DeLog_StoredPrograms")]
        [StringLength(120)]
        public string DeLogStoredPrograms { get; set; } = null!;

        [Required]
        [Column("DeLog_GroupID")]
        public Guid DeLogGroupId { get; set; }

        [Required]
        [Column("DeLog_isCustomDebug")]
        public bool DeLogIsCustomDebug { get; set; }

        [Required]
        [Column("DeLog_ExecutionProgram")]
        [StringLength(120)]
        public string DeLogExecutionProgram { get; set; } = null!;

        [Column("DeLog_ExecutionInfo")]
        public string? DeLogExecutionInfo { get; set; }

        [Column("DeLog_verifyNeeded")]
        public bool? DeLogVerifyNeeded { get; set; }

        [Required]
        [Column("DeLog_ExDateTime")]
        public DateTime DeLogExDateTime { get; set; }
    }
}
