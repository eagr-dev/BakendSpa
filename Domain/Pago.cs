using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSpa.Domain
{

    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id_pago")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPago { get; set; }

        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("preference_id")]
        [MaxLength(255)]
        public string? PreferenceId { get; set; }

        [Column("payment_id")]
        [MaxLength(255)]
        public string? PaymentId { get; set; }

        [Column("external_reference")]
        [MaxLength(255)]
        public string? ExternalReference { get; set; }

        [Column("monto", TypeName = "decimal(10,2)")]
        public decimal Monto { get; set; }

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "pending";

        [Column("collection_status")]
        [MaxLength(100)]
        public string? CollectionStatus { get; set; }

        [Column("payment_type")]
        [MaxLength(100)]
        public string? PaymentType { get; set; }

        [Column("processing_mode")]
        [MaxLength(100)]
        public string? ProcessingMode { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [ForeignKey("IdCita")]
        public Cita Cita { get; set; } = null!;
    }
}
