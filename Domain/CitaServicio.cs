using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BackendSpa.Domain
{

    [Table("cita_servicios")]
    public class CitaServicio
    {
        [Key]
        [Column("id_cita_servicio")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCitaServicio { get; set; }

        [Column("id_cita")]
        public int IdCita { get; set; }

        [Column("id_servicio")]
        public int IdServicio { get; set; }

        [Column("precio_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecioUnitario { get; set; }

        [ForeignKey("IdCita")]
        public Cita Cita { get; set; } = null!;

        [ForeignKey("IdServicio")]
        public Servicio Servicio { get; set; } = null!;
    }
}
