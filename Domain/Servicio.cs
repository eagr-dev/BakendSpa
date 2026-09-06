using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BackendSpa.Domain
{

    [Table("servicios")]
    public class Servicio
    {
        [Key]
        [Column("id_servicio")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdServicio { get; set; }

        [Column("id_categoria")]
        public int IdCategoria { get; set; }

        [Column("nombre")]
        [MaxLength(150)]
        [MinLength(2)]
        public string Nombre { get; set; } = string.Empty;

        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("duracion_min")]
        public int? DuracionMin { get; set; }

        [Column("precio", TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [ForeignKey("IdCategoria")]
        public Categoria Categoria { get; set; } = null!;

        public ICollection<CitaServicio> CitaServicios { get; set; } = new List<CitaServicio>();
    }
}
