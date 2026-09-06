using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendSpa.Domain
{

    [Table("citas")]
    public class Cita
    {
        [Key]
        [Column("id_cita")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCita { get; set; }

        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Column("fecha", TypeName = "date")]
        public DateTime Fecha { get; set; }

        [Column("hora_inicio", TypeName = "time")]
        public TimeSpan HoraInicio { get; set; }

        [Column("hora_fin", TypeName = "time")]
        public TimeSpan? HoraFin { get; set; }

        [Column("estado")]
        public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;

        [Column("precio_total", TypeName = "decimal(10,2)")]
        public decimal PrecioTotal { get; set; }

        [Column("anticipo", TypeName = "decimal(10,2)")]
        public decimal Anticipo { get; set; }

        [Column("creado_en")]
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;

        [ForeignKey("IdCliente")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<CitaServicio> CitaServicios { get; set; } = new List<CitaServicio>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    }
}
