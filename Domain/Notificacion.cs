using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSpa.Domain
{

    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("id_notificacion")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdNotificacion { get; set; }

        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("destinatario")]
        [MaxLength(20)]
        public string Destinatario { get; set; } = string.Empty;

        [Column("tipo")]
        public TipoNotificacion Tipo { get; set; }

        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Column("status")]
        [MaxLength(50)]
        public string Status { get; set; } = "pendiente";

        [Column("enviado_en")]
        public DateTime? EnviadoEn { get; set; }

        [Column("intentos")]
        public int Intentos { get; set; } = 0;

        [Column("errorDetalle")]
        public string? ErrorDetalle { get; set; }

        [ForeignKey("IdCita")]
        public Cita Cita { get; set; } = null!;
    }
}
